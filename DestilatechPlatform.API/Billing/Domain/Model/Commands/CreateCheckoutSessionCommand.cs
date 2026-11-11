namespace DestilatechPlatform.API.Billing.Domain.Model.Commands;

public record CreateCheckoutSessionCommand(int UserId, int PlanId);