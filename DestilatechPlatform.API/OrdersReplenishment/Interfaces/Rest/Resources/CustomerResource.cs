namespace DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Resources;

/// <summary>
/// Customer as returned by the API. The customer type uses the web app values, for example LIQUOR_STORE.
/// </summary>
public record CustomerResource(
    int Id,
    int UserId,
    string Name,
    string ContactName,
    string Phone,
    string Email,
    string Address,
    string CustomerType);
