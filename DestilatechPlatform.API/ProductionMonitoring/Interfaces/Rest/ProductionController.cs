using System.Security.Claims;
using System.Text.Json.Serialization;
using DestilatechPlatform.API.ProductionMonitoring.Application;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Aggregates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest;

[ApiController]
[Route("api/v1")]
[Tags("Production")]
public sealed class ProductionController(
    ProductionService production) : ControllerBase
{
    public sealed record BatchRequest(string Code, string Variety, DateTimeOffset StartDate,
        decimal EstimatedQuantity, string? Tank, string? Notes);

    public sealed record UpdateBatchRequest(string Stage, string Variety, decimal EstimatedQuantity,
        string? Tank, string? Notes, int? ProductId, int BottledUnits, string? StageNotes,
        bool ConfirmOverEstimate = false, decimal BottleVolumeLiters = 0.75m);

    public sealed record VariableRequest(int BatchId, string Type, decimal MinRange, decimal MaxRange,
        string? SensorId);

    public sealed record RangeRequest(decimal MinRange, decimal MaxRange);

    public sealed record ReadingRequest(int ProcessVariableId, decimal Value, DateTimeOffset RecordedAt,
        string? SourceKey = null);

    public sealed record BatchReadingRequest([property: JsonPropertyName("source_id")] string SourceId,
        [property: JsonPropertyName("source_key")] string SourceKey, decimal Value, DateTimeOffset RecordedAt);

    public sealed record StatusRequest(string Stage, string? Notes, int? ProductId, int BottledUnits,
        bool ConfirmOverEstimate = false, decimal BottleVolumeLiters = 0.75m);

    private int ResolveUserId(int requested)
    {
        if (User.Identity?.IsAuthenticated == true)
            return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"),
                out var id) && id > 0 ? id : 0;

        return requested > 0 ? requested : 0;
    }

    private static object BatchResource(ProductionBatch batch) => new
    {
        batch.Id, batch.UserId, batch.Code, batch.Variety, batch.StartDate, batch.EstimatedQuantity,
        batch.Tank, batch.Stage, batch.Notes, batch.ProductId, batch.BottledUnits,
        StageHistory = batch.StageHistory.OrderBy(x => x.ChangedAt).ThenBy(x => x.Id)
    };

    [HttpGet("batches")]
    public async Task<IActionResult> GetBatches([FromQuery] int userId, CancellationToken ct)
    {
        var owner = ResolveUserId(userId);
        if (owner == 0) return Unauthorized();

        var batches = await production.Batches(owner).OrderByDescending(x => x.StartDate).ToListAsync(ct);
        return Ok(batches.Select(BatchResource));
    }

    [HttpGet("process-variables")]
    public async Task<IActionResult> GetVariables([FromQuery] int userId, CancellationToken ct) =>
        ResolveUserId(userId) is var owner && owner > 0
            ? Ok(await production.Variables(owner).OrderBy(x => x.Id).ToListAsync(ct)) : Unauthorized();

    [HttpGet("sensor-readings")]
    public async Task<IActionResult> GetReadings([FromQuery] int userId, [FromQuery(Name = "_limit")] int? limit,
        CancellationToken ct) =>
        ResolveUserId(userId) is var owner && owner > 0
            ? Ok(await production.Readings(owner).OrderByDescending(x => x.RecordedAt).ThenByDescending(x => x.Id)
                .Take(Math.Clamp(limit ?? 500, 1, 500)).ToListAsync(ct)) : Unauthorized();

    [HttpPost("batches")]
    public async Task<IActionResult> CreateBatch([FromQuery] int userId, BatchRequest request, CancellationToken ct)
    {
        var owner = ResolveUserId(userId);
        if (owner == 0) return Unauthorized();

        try
        {
            var batch = new ProductionBatch(owner, request.Code, request.Variety, request.StartDate,
                request.EstimatedQuantity, request.Tank, request.Notes);
            return StatusCode(201, BatchResource(await production.CreateBatchAsync(batch, ct)));
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (InvalidOperationException ex) when (ex.Message == "The batch code already exists.")
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPut("batches/{id:int}")]
    public async Task<IActionResult> UpdateBatch([FromQuery] int userId, int id, UpdateBatchRequest request,
        CancellationToken ct)
    {
        var owner = ResolveUserId(userId);
        if (owner == 0) return Unauthorized();

        return await WriteBatchAsync(() => production.UpdateBatchAsync(owner, id, request.Stage, request.Variety,
            request.EstimatedQuantity, request.Tank, request.Notes, request.ProductId, request.BottledUnits,
            request.StageNotes, request.ConfirmOverEstimate, request.BottleVolumeLiters, ct));
    }

    private async Task<IActionResult> WriteBatchAsync(Func<Task<ProductionBatch?>> update)
    {
        try
        {
            var batch = await update();
            return batch is null ? NotFound() : Ok(BatchResource(batch));
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (DbUpdateConcurrencyException) { return Conflict("Batch or stock changed. Refresh and try again."); }
        catch (InvalidOperationException ex) when (ex.Message is "The batch is closed." or
            "Invalid batch stage transition.")
        {
            return UnprocessableEntity(ex.Message);
        }
        catch (InvalidOperationException ex) when (ex.Message is "Maximum stock exceeded." or
            "Bottled volume exceeds the estimate. Confirm to continue.")
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPost("process-variables")]
    public async Task<IActionResult> CreateVariable([FromQuery] int userId, VariableRequest request,
        CancellationToken ct)
    {
        var owner = ResolveUserId(userId);
        if (owner == 0) return Unauthorized();

        try
        {
            var variable = new ProcessVariable(owner, request.BatchId, request.Type, request.MinRange,
                request.MaxRange, request.SensorId);
            var saved = await production.AddVariableAsync(variable, ct);
            return saved is null ? NotFound() : StatusCode(201, saved);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (InvalidOperationException ex) when (ex.Message == "The variable already exists for this batch.")
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPatch("process-variables/{id:int}")]
    public async Task<IActionResult> UpdateRange([FromQuery] int userId, int id, RangeRequest request,
        CancellationToken ct)
    {
        var owner = ResolveUserId(userId);
        if (owner == 0) return Unauthorized();

        try
        {
            var variable = await production.UpdateRangeAsync(owner, id, request.MinRange, request.MaxRange, ct);
            return variable is null ? NotFound() : Ok(variable);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
    }

    [HttpPost("sensor-readings")]
    public async Task<IActionResult> RecordReading([FromQuery] int userId, ReadingRequest request,
        CancellationToken ct)
    {
        var owner = ResolveUserId(userId);
        if (owner == 0) return Unauthorized();

        try
        {
            var (reading, replayed) = await production.RecordReadingAsync(owner, request.ProcessVariableId,
                null, null, request.SourceKey, request.Value, request.RecordedAt, ct);
            return reading is null ? NotFound() : replayed ? Ok(reading) : StatusCode(201, reading);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (InvalidOperationException ex) when (ex.Message == "The batch is not in a monitored stage.")
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPatch("batches/{id:int}/status")]
    public async Task<IActionResult> UpdateStatus([FromQuery] int userId, int id, StatusRequest request,
        CancellationToken ct)
    {
        var owner = ResolveUserId(userId);
        if (owner == 0) return Unauthorized();

        return await WriteBatchAsync(() => production.UpdateBatchAsync(owner, id, request.Stage, "",
            0, null, null, request.ProductId, request.BottledUnits, request.Notes,
            request.ConfirmOverEstimate, request.BottleVolumeLiters, ct, updateDetails: false));
    }

    [HttpGet("batches/{id:int}/readings")]
    public async Task<IActionResult> GetBatchReadings([FromQuery] int userId, int id, [FromQuery] string? variable,
        [FromQuery] DateTimeOffset? from, [FromQuery] DateTimeOffset? to, CancellationToken ct)
    {
        var owner = ResolveUserId(userId);
        if (owner == 0) return Unauthorized();

        if (from.HasValue && to.HasValue && from.Value > to.Value)
            return BadRequest("The start must be before the end.");

        if (!await production.BatchExistsAsync(owner, id, ct)) return NotFound();

        var readings = production.Readings(owner).Where(x => x.BatchId == id);
        if (!string.IsNullOrWhiteSpace(variable))
        {
            var type = variable.Trim().ToUpperInvariant();
            if (type == "TEMPERATURA") type = "TEMPERATURE";

            var variables = production.Variables(owner).Where(x => x.BatchId == id && x.Type == type);
            readings = readings.Where(x => variables.Any(v => v.Id == x.ProcessVariableId));
        }

        if (from is DateTimeOffset start) readings = readings.Where(x => x.RecordedAt >= start.UtcDateTime);
        if (to is DateTimeOffset end) readings = readings.Where(x => x.RecordedAt <= end.UtcDateTime);

        return Ok(await readings.OrderBy(x => x.RecordedAt).ThenBy(x => x.Id).Take(1000).ToListAsync(ct));
    }

    [HttpPost("batches/{id:int}/readings")]
    public async Task<IActionResult> IngestBatchReading([FromQuery] int userId, int id, BatchReadingRequest request,
        CancellationToken ct)
    {
        var owner = ResolveUserId(userId);
        if (owner == 0) return Unauthorized();

        if (string.IsNullOrWhiteSpace(request.SourceId) || string.IsNullOrWhiteSpace(request.SourceKey))
            return BadRequest("Source ID and source key are required.");

        try
        {
            var (reading, replayed) = await production.RecordReadingAsync(owner, null, id, request.SourceId.Trim(),
                request.SourceKey, request.Value, request.RecordedAt, ct);
            return reading is null ? NotFound() : replayed ? Ok(reading) : StatusCode(201, reading);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (InvalidOperationException ex) when (ex.Message == "The batch is not in a monitored stage.")
        {
            return Conflict(ex.Message);
        }
    }
}