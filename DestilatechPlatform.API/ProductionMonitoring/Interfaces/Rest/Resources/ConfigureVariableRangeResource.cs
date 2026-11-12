namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Resources;

public record ConfigureVariableRangeResource(
    int TenantId,
    decimal MinRange, decimal MaxRange);
