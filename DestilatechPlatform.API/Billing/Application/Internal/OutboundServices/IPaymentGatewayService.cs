using DestilatechPlatform.API.Billing.Domain.Model.Aggregates;
using DestilatechPlatform.API.Billing.Domain.Model.Errors;
using DestilatechPlatform.API.Shared.Application.Model;

namespace DestilatechPlatform.API.Billing.Application.Internal.OutboundServices;

public record CheckoutSessionInfo(string SessionId, string Url);

public record PaidSessionInfo(
    string SessionId,
    int UserId,
    int PlanId,
    decimal Amount,
    string Currency,
    string? CardBrand,
    string? CardLast4,
    DateTimeOffset PaidAt);

public interface IPaymentGatewayService
{
    Task<Result<CheckoutSessionInfo>> CreateCheckoutSessionAsync(int userId, Plan plan,
        CancellationToken cancellationToken = default);

    Task<Result<PaidSessionInfo>> GetPaidSessionAsync(string sessionId,
        CancellationToken cancellationToken = default);
}