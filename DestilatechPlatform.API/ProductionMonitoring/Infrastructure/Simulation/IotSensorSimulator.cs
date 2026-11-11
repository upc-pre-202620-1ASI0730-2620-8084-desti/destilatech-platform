using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace DestilatechPlatform.API.ProductionMonitoring.Infrastructure.Simulation;

/// <summary>
/// Simulates IoT sensors (US64): every interval it sends a synthetic reading for each process variable
/// with a sensor of a batch in a monitored stage, to <c>POST /batches/{id}/readings</c>.
/// </summary>
/// <remarks>
/// Each reading has a source_id (the sensor) and a source_key (the reading). After a temporary error the same
/// payload is sent again with the same source_key, so the API does not register the reading twice.
/// </remarks>
public class IotSensorSimulator(
    IHttpClientFactory httpClientFactory,
    IOptions<IotSensorSimulatorOptions> options,
    ILogger<IotSensorSimulator> logger) : BackgroundService
{
    private static readonly string[] MonitoredStages = ["FERMENTATION", "DISTILLATION", "RESTING"];
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IotSensorSimulatorOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled) return;

        var random = _options.Seed is int seed ? new Random(seed) : new Random();
        var client = httpClientFactory.CreateClient(nameof(IotSensorSimulator));
        client.BaseAddress = new Uri(_options.ApiBaseUrl.TrimEnd('/') + "/");
        logger.LogInformation("IoT sensor simulator started for account {UserId} every {Seconds} s",
            _options.UserId, _options.IntervalSeconds);

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(Math.Max(1, _options.IntervalSeconds)));
        do
        {
            try
            {
                await SendRoundAsync(client, random, stoppingToken);
            }
            catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException
                                                  && !stoppingToken.IsCancellationRequested)
            {
                logger.LogWarning("IoT sensor simulator could not reach the API: {Message}", exception.Message);
            }
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task SendRoundAsync(HttpClient client, Random random, CancellationToken cancellationToken)
    {
        var batches = await client.GetFromJsonAsync<List<SimulatedBatch>>(
            $"batches?userId={_options.UserId}", JsonOptions, cancellationToken) ?? [];
        var variables = await client.GetFromJsonAsync<List<SimulatedVariable>>(
            $"process-variables?userId={_options.UserId}", JsonOptions, cancellationToken) ?? [];

        var monitoredBatchIds = batches
            .Where(batch => MonitoredStages.Contains(batch.Stage))
            .Where(batch => _options.BatchId is null || batch.Id == _options.BatchId)
            .Select(batch => batch.Id)
            .ToHashSet();
        var targets = variables
            .Where(variable => variable.SensorId is not null && monitoredBatchIds.Contains(variable.BatchId))
            .Where(variable => string.IsNullOrWhiteSpace(_options.VariableType) ||
                               string.Equals(variable.Type, _options.VariableType, StringComparison.OrdinalIgnoreCase));

        foreach (var variable in targets)
        {
            var reading = new SimulatedReading(variable.SensorId!, $"sim-{variable.Id}-{DateTime.UtcNow:yyyyMMddHHmmss}",
                NextValue(variable, random), DateTimeOffset.UtcNow);
            await SendWithRetryAsync(client, variable.BatchId, reading, cancellationToken);
        }
    }

    private async Task SendWithRetryAsync(HttpClient client, int batchId, SimulatedReading reading,
        CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt <= _options.MaxRetries; attempt++)
        {
            try
            {
                var response = await client.PostAsJsonAsync($"batches/{batchId}/readings?userId={_options.UserId}",
                    reading, JsonOptions, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    logger.LogInformation("Reading {SourceKey} = {Value} sent to batch {BatchId} ({Status})",
                        reading.SourceKey, reading.Value, batchId, (int)response.StatusCode);
                    return;
                }

                if (!IsTemporary(response.StatusCode))
                {
                    logger.LogWarning("Reading {SourceKey} rejected with {Status}", reading.SourceKey,
                        (int)response.StatusCode);
                    return;
                }
            }
            catch (HttpRequestException exception)
            {
                logger.LogWarning("Reading {SourceKey} failed: {Message}", reading.SourceKey, exception.Message);
            }

            // Same payload and same source_key, so a retry never duplicates the reading.
            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)), cancellationToken);
        }

        logger.LogWarning("Reading {SourceKey} was not sent after {Retries} retries", reading.SourceKey,
            _options.MaxRetries);
    }

    private decimal NextValue(SimulatedVariable variable, Random random)
    {
        var width = variable.MaxRange - variable.MinRange;
        var value = random.NextDouble() < _options.AnomalyProbability
            ? variable.MaxRange + width * (decimal)(0.1 + random.NextDouble() * 0.2)
            : variable.MinRange + width * (decimal)random.NextDouble();
        return decimal.Round(value, 2);
    }

    private static bool IsTemporary(HttpStatusCode statusCode)
    {
        return statusCode is HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests
            || (int)statusCode >= 500;
    }

    private record SimulatedBatch(int Id, string Stage);

    private record SimulatedVariable(int Id, int BatchId, string Type, decimal MinRange, decimal MaxRange,
        string? SensorId);

    private record SimulatedReading(
        [property: JsonPropertyName("source_id")] string SourceId,
        [property: JsonPropertyName("source_key")] string SourceKey,
        decimal Value,
        DateTimeOffset RecordedAt);
}
