using DestilatechPlatform.API.Billing.Application.CommandServices;
using DestilatechPlatform.API.Billing.Application.Internal.OutboundServices;
using DestilatechPlatform.API.Billing.Domain.Model.Aggregates;
using DestilatechPlatform.API.Billing.Domain.Model.Commands;
using DestilatechPlatform.API.Billing.Domain.Model.Errors;
using DestilatechPlatform.API.Billing.Domain.Model.ValueObjects;
using DestilatechPlatform.API.Billing.Domain.Repositories;
using DestilatechPlatform.API.Iam.Interfaces.Acl;
using DestilatechPlatform.API.Shared.Application.Patterns;
using DestilatechPlatform.API.Shared.Domain.Repositories;

namespace DestilatechPlatform.API.Billing.Application.Internal.CommandServices;

public class CheckoutCommandService(
    IPlanRepository planRepository,
    IPaymentRepository paymentRepository,
    ISubscriptionRepository subscriptionRepository,
    IPaymentGatewayService paymentGateway,
    IIamContextFacade iamContextFacade,
    IUnitOfWork unitOfWork) : ICheckoutCommandService
{
    private const string ExpectedCurrency = "PEN";

    public async Task<Result<CheckoutSessionInfo, BillingError>> Handle(CreateCheckoutSessionCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.UserId <= 0 || command.PlanId <= 0)
            return new Result<CheckoutSessionInfo, BillingError>.Failure(BillingError.InvalidData);

        if (!await iamContextFacade.UserExistsAsync(command.UserId, cancellationToken))
            return new Result<CheckoutSessionInfo, BillingError>.Failure(BillingError.UserNotFound);

        var plan = await planRepository.FindByIdAsync(command.PlanId, cancellationToken);
        if (plan is null)
            return new Result<CheckoutSessionInfo, BillingError>.Failure(BillingError.PlanNotFound);

        // El precio sale del plan guardado en la base de datos, nunca del cliente
        return await paymentGateway.CreateCheckoutSessionAsync(command.UserId, plan, cancellationToken);
    }

    public async Task<Result<ConfirmationResult, BillingError>> Handle(ConfirmCheckoutSessionCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.SessionId))
            return new Result<ConfirmationResult, BillingError>.Failure(BillingError.InvalidData);

        // Idempotencia: si esta sesión ya se confirmó, devuelve el mismo resultado
        var existingPayment = await paymentRepository.FindByReferenceAsync(command.SessionId, cancellationToken);
        if (existingPayment is not null)
        {
            var existingSubscription =
                await subscriptionRepository.FindByPaymentReferenceAsync(command.SessionId, cancellationToken);
            if (existingSubscription is not null)
                return new Result<ConfirmationResult, BillingError>.Success(
                    new ConfirmationResult(existingPayment, existingSubscription));
        }

        // Se le pregunta a Stripe: el cliente no decide si el pago fue válido
        var gatewayResult = await paymentGateway.GetPaidSessionAsync(command.SessionId, cancellationToken);
        if (gatewayResult is Result<PaidSessionInfo, BillingError>.Failure failure)
            return new Result<ConfirmationResult, BillingError>.Failure(failure.Error);
        var session = ((Result<PaidSessionInfo, BillingError>.Success)gatewayResult).Value;

        var plan = await planRepository.FindByIdAsync(session.PlanId, cancellationToken);
        if (plan is null)
            return new Result<ConfirmationResult, BillingError>.Failure(BillingError.PlanNotFound);

        // Lo que cobró Stripe debe coincidir con el precio del plan
        if (session.Amount != plan.Price ||
            !string.Equals(session.Currency, ExpectedCurrency, StringComparison.OrdinalIgnoreCase))
            return new Result<ConfirmationResult, BillingError>.Failure(BillingError.AmountMismatch);

        if (!await iamContextFacade.UserExistsAsync(session.UserId, cancellationToken))
            return new Result<ConfirmationResult, BillingError>.Failure(BillingError.UserNotFound);

        var payment = new Payment(session.UserId, plan.Id, session.Amount, ExpectedCurrency,
            EPaymentProvider.Stripe, session.SessionId, EPaymentStatus.Paid, session.CardBrand,
            session.CardLast4, session.PaidAt);
        var subscription = new Subscription(session.UserId, plan.Id, session.SessionId, session.PaidAt);

        await paymentRepository.AddAsync(payment, cancellationToken);
        await subscriptionRepository.AddAsync(subscription, cancellationToken);
        await iamContextFacade.ExtendUserAccessAsync(session.UserId, subscription.RenewalDate, cancellationToken);

        // Un solo guardado: pago, suscripción y acceso de IAM, todo o nada
        await unitOfWork.CompleteAsync(cancellationToken);

        return new Result<ConfirmationResult, BillingError>.Success(new ConfirmationResult(payment, subscription));
    }
}