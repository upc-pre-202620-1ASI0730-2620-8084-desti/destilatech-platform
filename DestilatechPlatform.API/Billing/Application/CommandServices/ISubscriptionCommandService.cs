using DestilatechPlatform.API.Billing.Domain.Model.Aggregates;
using DestilatechPlatform.API.Billing.Domain.Model.Commands;
using DestilatechPlatform.API.Billing.Domain.Model.Errors;
using DestilatechPlatform.API.Shared.Application.Model;

namespace DestilatechPlatform.API.Billing.Application.CommandServices;

public interface ISubscriptionCommandService
{
    Task<Result<Subscription>> Handle(CancelSubscriptionCommand command,
        CancellationToken cancellationToken = default);
}