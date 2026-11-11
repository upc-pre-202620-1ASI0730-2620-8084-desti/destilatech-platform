namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Resources;

public record BatchStageChangeResource(int Id, string Stage, DateTime ChangedAt, string Notes);
