namespace DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Entities;

public sealed class BatchStageChange
{
    public int Id { get; private set; }
    public string Stage { get; private set; } = "";
    public DateTime ChangedAt { get; private set; }
    public string Notes { get; private set; } = "";

    private BatchStageChange() { }

    public BatchStageChange(string stage, DateTime changedAt, string notes)
    {
        Stage = stage;
        ChangedAt = changedAt;
        Notes = notes;
    }
}