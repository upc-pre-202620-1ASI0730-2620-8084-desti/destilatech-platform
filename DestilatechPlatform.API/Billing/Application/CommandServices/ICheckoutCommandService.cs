using DestilatechPlatform.API.Billing.Application.Internal.OutboundServices;
using DestilatechPlatform.API.Billing.Domain.Model.Aggregates;
using DestilatechPlatform.API.Billing.Domain.Model.Commands;
using DestilatechPlatform.API.Billing.Domain.Model.Errors;
using DestilatechPlatform.API.Shared.Application.Model;

namespace DestilatechPlatform.API.Billing.Application.CommandServices;

public record ConfirmationResult(Payment Payment, Subscription Subscription);

public interface ICheckoutCommandService
{
    Task<Result<CheckoutSessionInfo>> Handle(CreateCheckoutSessionCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<ConfirmationResult>> Handle(ConfirmCheckoutSessionCommand command,
        CancellationToken cancellationToken = default);
}