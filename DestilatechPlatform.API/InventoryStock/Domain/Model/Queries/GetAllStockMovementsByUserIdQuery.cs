namespace DestilatechPlatform.API.InventoryStock.Domain.Model.Queries;

/// <summary>
/// Lists the stock movements of the account, newest first.
/// </summary>
public record GetAllStockMovementsByUserIdQuery(int UserId);
