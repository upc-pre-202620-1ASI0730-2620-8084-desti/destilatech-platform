namespace DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Resources;

/// <summary>
/// Data to request a replenishment order. The account can come in the body or in the <c>userId</c> query parameter.
/// </summary>
public record CreateReplenishmentOrderResource(
    int? UserId,
    string SupplierName,
    string? SupplierContact,
    int ProductId,
    int Quantity,
    DateTimeOffset? ExpectedDate,
    string? Notes);
