namespace DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Resources;

/// <summary>
/// New status of a replenishment order: RECEIVED or CANCELLED.
/// </summary>
public record UpdateReplenishmentOrderStatusResource(
    int TenantId,
    string Status);
