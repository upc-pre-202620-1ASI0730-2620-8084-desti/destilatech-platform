namespace DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Resources;

/// <summary>
/// Data to register a customer. The account can come in the body or in the <c>userId</c> query parameter.
/// </summary>
public record CreateCustomerResource(
    int TenantId,
    string Name,
    string? ContactName,
    string? Phone,
    string? Email,
    string? Address,
    string CustomerType);
