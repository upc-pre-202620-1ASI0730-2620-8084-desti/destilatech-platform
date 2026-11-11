using System.Text.Json.Serialization;

namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Resources;

/// <summary>
/// Reading sent by an IoT device for a batch, identified by the sensor (source_id) and the reading key (source_key).
/// </summary>
public record IngestBatchReadingResource(
    [property: JsonPropertyName("source_id")] string SourceId,
    [property: JsonPropertyName("source_key")] string SourceKey,
    decimal Value,
    DateTimeOffset RecordedAt);
