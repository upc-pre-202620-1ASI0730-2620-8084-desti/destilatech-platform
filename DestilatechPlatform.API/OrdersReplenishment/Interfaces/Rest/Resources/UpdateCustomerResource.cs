namespace DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Resources;

/// <summary>
/// New data of a customer. The account can come in the body or in the <c>userId</c> query parameter.
/// </summary>
public record UpdateCustomerResource(
    int TenantId,
    string Name,
    string? ContactName,
    string? Phone,
    string? Email,
    string? Address,
    string CustomerType);
