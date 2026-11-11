using DestilatechPlatform.API.Shared.Domain.Model.Errors;

namespace DestilatechPlatform.API.InventoryStock.Domain.Model;

/// <summary>
/// Business error of the Inventory &amp; Stock bounded context.
/// </summary>
/// <param name="Code">Stable error code, in kebab case.</param>
/// <param name="Message">English description of the error.</param>
/// <param name="Type">Kind of error, used by the REST layer to choose the HTTP status code.</param>
public sealed record InventoryStockError(string Code, string Message, EErrorType Type = EErrorType.Validation)
    : Error(Code, Message, Type)
{
    public static InventoryStockError InvalidData(string message) =>
        new("invalid-data", message);

    public static InventoryStockError ProductNotFound(int productId) =>
        new("product-not-found", $"Product {productId} was not found.", EErrorType.NotFound);

    public static InventoryStockError StockItemNotFound(int stockItemId) =>
        new("stock-item-not-found", $"Stock item {stockItemId} was not found.", EErrorType.NotFound);

    public static InventoryStockError SourceWorkflowRequired() =>
        new("source-workflow-required", "Use the source workflow for this movement.");

    public static InventoryStockError InsufficientStock() =>
        new("insufficient-stock", "Insufficient stock.", EErrorType.Conflict);

    public static InventoryStockError MaximumStockExceeded() =>
        new("maximum-stock-exceeded", "Maximum stock exceeded.", EErrorType.Conflict);

    public static InventoryStockError StockChanged() =>
        new("stock-changed", "Stock changed. Refresh and try again.", EErrorType.Conflict);
}
