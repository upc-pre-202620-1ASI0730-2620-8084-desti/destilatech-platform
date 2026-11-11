namespace DestilatechPlatform.API.Billing.Domain.Model.Errors;

public enum BillingError
{
    InvalidData,
    UserNotFound,
    PlanNotFound,
    SubscriptionNotFound,
    SessionNotFound,
    PaymentNotCompleted,
    AmountMismatch,
    PaymentGatewayFailure
}