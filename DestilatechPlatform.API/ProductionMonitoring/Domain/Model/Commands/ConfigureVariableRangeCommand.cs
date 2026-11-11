namespace DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Commands;

/// <summary>
/// Changes the allowed range of a process variable.
/// </summary>
public record ConfigureVariableRangeCommand(int UserId, int ProcessVariableId, decimal MinRange, decimal MaxRange);
