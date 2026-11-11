namespace DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Queries;

/// <summary>
/// Lists the process variables of the account.
/// </summary>
public record GetAllProcessVariablesByUserIdQuery(int UserId);
