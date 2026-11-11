namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Model.ValueObjects;

/// <summary>
/// Kind of business or person that buys from the producer or trader.
/// </summary>
public enum CustomerType
{
    LiquorStore,
    Distributor,
    Restaurant,
    Market,
    FinalConsumer
}
