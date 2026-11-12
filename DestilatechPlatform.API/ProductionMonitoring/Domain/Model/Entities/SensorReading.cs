using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Aggregates;

namespace DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Entities;

public sealed partial class SensorReading
{
    public int Id { get; private set; }
    public int ProcessVariableId { get; private set; }
    public int BatchId { get; private set; }
    public int UserId { get; private set; }
    public decimal Value { get; private set; }
    public string? SourceKey { get; private set; }
    public bool WithinRange { get; private set; }
    public DateTime RecordedAt { get; private set; }

    private SensorReading() { }

    public SensorReading(ProcessVariable variable, decimal value, DateTimeOffset recordedAt, string? sourceKey)
    {
        if (value <= -100_000m || value >= 100_000m || decimal.Round(value, 4) != value)
            throw new ArgumentException("Invalid sensor value.");

        if (recordedAt == default || recordedAt > DateTimeOffset.UtcNow.AddMinutes(1))
            throw new ArgumentException("Invalid reading timestamp.");

        UserId = variable.UserId;
        BatchId = variable.BatchId;
        ProcessVariableId = variable.Id;
        SourceKey = sourceKey;
        Value = value;
        WithinRange = variable.IsWithinRange(value);
        RecordedAt = recordedAt.UtcDateTime;
    }
}