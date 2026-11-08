using DestilatechPlatform.API.Billing.Application.CommandServices;
using DestilatechPlatform.API.Billing.Domain.Model.Aggregates;
using DestilatechPlatform.API.Billing.Domain.Model.Commands;
using DestilatechPlatform.API.Billing.Domain.Model.Errors;
using DestilatechPlatform.API.Billing.Domain.Repositories;
using DestilatechPlatform.API.Shared.Application.Patterns;
using DestilatechPlatform.API.Shared.Domain.Repositories;

namespace DestilatechPlatform.API.Billing.Application.Internal.CommandServices;

public class SubscriptionCommandService(ISubscriptionRepository subscriptionRepository, IUnitOfWork unitOfWork)
    : ISubscriptionCommandService
{
    public async Task<Result<Subscription, BillingError>> Handle(CancelSubscriptionCommand command,
        CancellationToken cancellationToken = default)
    {
        var subscription = await subscriptionRepository.FindByIdAsync(command.SubscriptionId, cancellationToken);
        if (subscription is null)
            return new Result<Subscription, BillingError>.Failure(BillingError.SubscriptionNotFound);

        subscription.Cancel();
        subscriptionRepository.Update(subscription);
        await unitOfWork.CompleteAsync(cancellationToken);

        return new Result<Subscription, BillingError>.Success(subscription);
    }
}