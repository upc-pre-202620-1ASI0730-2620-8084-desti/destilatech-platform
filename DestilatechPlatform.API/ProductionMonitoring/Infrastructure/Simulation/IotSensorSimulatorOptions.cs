namespace DestilatechPlatform.API.ProductionMonitoring.Infrastructure.Simulation;

/// <summary>
/// Settings of the IoT sensor simulator (US64), read from the <c>IotSensorSimulator</c> section.
/// </summary>
public class IotSensorSimulatorOptions
{
    public const string SectionName = "IotSensorSimulator";

    /// <summary>The simulator only runs when this is true.</summary>
    public bool Enabled { get; set; }

    /// <summary>Base URL of the API that receives the readings, for example http://localhost:5273/api/v1.</summary>
    public string ApiBaseUrl { get; set; } = "http://localhost:5273/api/v1";

    /// <summary>Account whose batches are simulated.</summary>
    public int UserId { get; set; } = 1;

    /// <summary>Optional batch to simulate. When empty, every batch in a monitored stage is simulated.</summary>
    public int? BatchId { get; set; }

    /// <summary>Optional metric to simulate (TEMPERATURE, PH, BRIX…). When empty, every metric is simulated.</summary>
    public string? VariableType { get; set; }

    /// <summary>Seconds between two rounds of readings.</summary>
    public int IntervalSeconds { get; set; } = 10;

    /// <summary>Probability (0 to 1) of sending a value out of the allowed range.</summary>
    public double AnomalyProbability { get; set; } = 0.05;

    /// <summary>Times a reading is sent again after a temporary error.</summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>Optional seed for repeatable values.</summary>
    public int? Seed { get; set; }
}
