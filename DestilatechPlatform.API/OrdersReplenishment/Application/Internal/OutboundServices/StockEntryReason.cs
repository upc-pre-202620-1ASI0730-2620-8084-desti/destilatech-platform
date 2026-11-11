namespace DestilatechPlatform.API.OrdersReplenishment.Application.Internal.OutboundServices;

/// <summary>
/// Why Orders &amp; Replenishment adds units back to stock.
/// </summary>
public enum StockEntryReason
{
    /// <summary>A cancelled order returns its products.</summary>
    Return,

    /// <summary>A received replenishment order brings new units.</summary>
    Purchase
}
