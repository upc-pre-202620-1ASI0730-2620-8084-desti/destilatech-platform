namespace DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Queries;

/// <summary>
/// Lists the batches of the account with their stage history, newest first.
/// </summary>
public record GetAllProductionBatchesByUserIdQuery(int UserId);
