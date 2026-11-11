namespace DestilatechPlatform.API.InventoryStock.Domain.Model.Commands;

/// <summary>
/// Configures the quantity at or below which a stock item raises a low stock alert.
/// </summary>
public record ConfigureLowStockThresholdCommand(int UserId, int StockItemId, decimal LowStockThreshold);
