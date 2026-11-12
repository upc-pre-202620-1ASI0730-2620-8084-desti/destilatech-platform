namespace DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Resources;

/// <summary>
/// New status of an order: its next status in the flow, or CANCELLED.
/// </summary>
public record UpdateOrderStatusResource(
    int TenantId,
    string Status);
