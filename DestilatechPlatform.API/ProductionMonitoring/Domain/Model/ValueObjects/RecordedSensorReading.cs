using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Entities;

namespace DestilatechPlatform.API.ProductionMonitoring.Domain.Model.ValueObjects;

/// <summary>
/// Result of recording a reading. Replayed is true when the reading with the same source key already existed.
/// </summary>
public record RecordedSensorReading(SensorReading Reading, bool Replayed);
