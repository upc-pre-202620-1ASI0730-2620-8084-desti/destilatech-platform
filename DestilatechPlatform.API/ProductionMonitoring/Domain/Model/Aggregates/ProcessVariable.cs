using DestilatechPlatform.API.Shared.Domain.Model.Aggregates.Internal;

namespace DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Aggregates;

public sealed partial class ProcessVariable : AggregateRoot
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public int BatchId { get; private set; }
    public string Type { get; private set; } = "";
    public decimal MinRange { get; private set; }
    public decimal MaxRange { get; private set; }
    public string? SensorId { get; private set; }

    private ProcessVariable() { }

    public ProcessVariable(int userId, int batchId, string type, decimal min, decimal max, string? sensorId)
    {
        if (userId <= 0 || batchId <= 0)
            throw new ArgumentException("Invalid account or batch.");

        if (type is not ("TEMPERATURE" or "PH" or "HUMIDITY" or "DENSITY" or "BRIX" or "ALCOHOL"))
            throw new ArgumentException("Unsupported variable type.");

        if (sensorId?.Length > 80)
            throw new ArgumentException("Sensor identifier is too long.");

        UserId = userId;
        BatchId = batchId;
        Type = type;
        SensorId = string.IsNullOrWhiteSpace(sensorId) ? null : sensorId.Trim();
        ConfigureRange(min, max);
    }

    public void ConfigureRange(decimal min, decimal max)
    {
        if (min >= max || min <= -100_000m || max >= 100_000m ||
            decimal.Round(min, 4) != min || decimal.Round(max, 4) != max)
            throw new ArgumentException("Set a valid range with the minimum below the maximum.");

        MinRange = min;
        MaxRange = max;
    }

    public bool IsWithinRange(decimal value) => value >= MinRange && value <= MaxRange;
}