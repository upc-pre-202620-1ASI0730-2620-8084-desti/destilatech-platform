namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.ACL;

/// <summary>
/// Entry point of Production &amp; Monitoring for the other bounded contexts.
/// </summary>
public interface IProductionContextFacade
{
    /// <summary>
    /// Batches of the account that are not bottled yet, newest first.
    /// </summary>
    Task<IReadOnlyCollection<ProductionBatchSnapshot>> FetchActiveBatchesAsync(int userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Batches of the account started on or after the given UTC date, oldest first.
    /// </summary>
    Task<IReadOnlyCollection<ProductionBatchSnapshot>> FetchBatchesStartedSinceAsync(int userId, DateTime sinceUtc,
        CancellationToken cancellationToken = default);
}
