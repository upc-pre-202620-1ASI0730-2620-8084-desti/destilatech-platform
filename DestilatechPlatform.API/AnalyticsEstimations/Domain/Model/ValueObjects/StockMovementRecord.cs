namespace DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.ValueObjects;

/// <summary>
/// Stock movement read from Inventory &amp; Stock Management, used as input for estimates and indicators.
/// </summary>
/// <param name="ProductId">Product that moved.</param>
/// <param name="Direction">Entry or exit.</param>
/// <param name="Reason">Reason given by Inventory, for example SALE, ORDER, WASTE or PURCHASE.</param>
/// <param name="Quantity">Units moved. Always positive.</param>
/// <param name="Date">When the movement happened.</param>
public readonly record struct StockMovementRecord(
    int ProductId,
    MovementDirection Direction,
    string Reason,
    decimal Quantity,
    DateTimeOffset Date)
{
    /// <summary>
    /// Quantity with sign: positive for entries, negative for exits.
    /// </summary>
    public decimal SignedQuantity => Direction == MovementDirection.In ? Quantity : -Quantity;
}
