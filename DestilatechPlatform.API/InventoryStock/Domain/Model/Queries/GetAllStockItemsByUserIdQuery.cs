namespace DestilatechPlatform.API.InventoryStock.Domain.Model.Queries;

/// <summary>
/// Lists the stock items of the account.
/// </summary>
public record GetAllStockItemsByUserIdQuery(int UserId);
