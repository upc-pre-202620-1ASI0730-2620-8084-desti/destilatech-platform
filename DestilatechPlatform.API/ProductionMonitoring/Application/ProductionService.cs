using DestilatechPlatform.API.AlertsNotifications.Interfaces.ACL;
using DestilatechPlatform.API.InventoryStock.Application;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Aggregates;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Entities;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.ProductionMonitoring.Application;

public sealed class ProductionService(AppDbContext db, InventoryService inventory, IAlertsContextFacade alerts)
{
    public IQueryable<ProductionBatch> Batches(int owner) =>
        db.Set<ProductionBatch>().AsNoTracking().Where(x => x.UserId == owner).Include(x => x.StageHistory);

    public IQueryable<ProcessVariable> Variables(int owner) =>
        db.Set<ProcessVariable>().AsNoTracking().Where(x => x.UserId == owner);

    public IQueryable<SensorReading> Readings(int owner) =>
        db.Set<SensorReading>().AsNoTracking().Where(x => x.UserId == owner);

    public async Task<ProductionBatch> CreateBatchAsync(ProductionBatch batch, CancellationToken ct)
    {
        if (await Batches(batch.UserId).AnyAsync(x => x.Code == batch.Code, ct))
            throw new InvalidOperationException("The batch code already exists.");

        db.Add(batch);
        await db.SaveChangesAsync(ct);
        return batch;
    }

    public async Task<ProductionBatch?> UpdateBatchAsync(int owner, int id, string stage, string variety,
        decimal quantity, string? tank, string? notes, int? productId, int bottledUnits, string? stageNotes,
        bool confirmOverEstimate, decimal bottleVolumeLiters, CancellationToken ct, bool updateDetails = true)
    {
        var batch = await db.Set<ProductionBatch>().Include(x => x.StageHistory)
            .SingleOrDefaultAsync(x => x.UserId == owner && x.Id == id, ct);

        if (batch is null) return null;

        if (stage == batch.Stage && stage == "BOTTLED")
        {
            if (batch.ProductId == productId && batch.BottledUnits == bottledUnits) return batch;
            throw new InvalidOperationException("The batch is closed.");
        }

        if (!updateDetails && stage == batch.Stage)
            throw new InvalidOperationException("Invalid batch stage transition.");

        if (updateDetails)
            batch.Edit(variety, quantity, tank, notes, recordChange: stage == batch.Stage);

        if (batch.Stage != stage)
        {
            if (stage == "BOTTLED")
            {
                if (bottleVolumeLiters <= 0 || bottleVolumeLiters > 10)
                    throw new ArgumentException("Invalid volume per bottle.");

                if (!confirmOverEstimate && bottledUnits * bottleVolumeLiters > batch.EstimatedQuantity)
                    throw new InvalidOperationException("Bottled volume exceeds the estimate. Confirm to continue.");
            }

            batch.Advance(stage, productId, bottledUnits, stageNotes);

            if (stage == "BOTTLED" &&
                !await inventory.StageBottledStockAsync(owner, batch.ProductId!.Value, bottledUnits, batch.Code, ct))
                throw new ArgumentException("Select an active bottled product from your inventory.");
        }

        await db.SaveChangesAsync(ct);
        return batch;
    }

    public async Task<ProcessVariable?> AddVariableAsync(ProcessVariable variable, CancellationToken ct)
    {
        if (!await Batches(variable.UserId).AnyAsync(x => x.Id == variable.BatchId, ct))
            return null;

        if (await Variables(variable.UserId).AnyAsync(x => x.BatchId == variable.BatchId &&
                x.Type == variable.Type, ct))
            throw new InvalidOperationException("The variable already exists for this batch.");

        db.Add(variable);
        await db.SaveChangesAsync(ct);
        return variable;
    }

    public async Task<ProcessVariable?> UpdateRangeAsync(int owner, int id, decimal min, decimal max,
        CancellationToken ct)
    {
        var variable = await db.Set<ProcessVariable>()
            .SingleOrDefaultAsync(x => x.UserId == owner && x.Id == id, ct);

        if (variable is null) return null;

        variable.ConfigureRange(min, max);
        await db.SaveChangesAsync(ct);
        return variable;
    }

    public Task<bool> BatchExistsAsync(int owner, int id, CancellationToken ct) =>
        db.Set<ProductionBatch>().AnyAsync(x => x.UserId == owner && x.Id == id, ct);

    public async Task<(SensorReading? Reading, bool Replayed)> RecordReadingAsync(int owner,
        int? variableId, int? batchId, string? sourceId, string? sourceKey, decimal value,
        DateTimeOffset recordedAt, CancellationToken ct)
    {
        var query = Variables(owner);

        if (batchId is int batchFilter) query = query.Where(x => x.BatchId == batchFilter);
        if (variableId is int variableFilter) query = query.Where(x => x.Id == variableFilter);
        if (sourceId is not null) query = query.Where(x => x.SensorId == sourceId);

        var variable = await query.SingleOrDefaultAsync(ct);
        if (variable is null) return (null, false);

        var key = string.IsNullOrWhiteSpace(sourceKey) ? null : sourceKey.Trim();
        if (key?.Length > 80) throw new ArgumentException("Source key is too long.");

        if (key is not null)
        {
            var existing = await Readings(owner)
                .SingleOrDefaultAsync(x => x.ProcessVariableId == variable.Id && x.SourceKey == key, ct);
            if (existing is not null) return (existing, true);
        }

        var batch = await db.Set<ProductionBatch>().AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == variable.BatchId && x.UserId == owner, ct);

        if (batch is null) return (null, false);

        if (batch.Stage is not ("FERMENTATION" or "DISTILLATION" or "RESTING"))
            throw new InvalidOperationException("The batch is not in a monitored stage.");

        var reading = new SensorReading(variable, value, recordedAt, key);
        db.Add(reading);

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException) when (key is not null && !ct.IsCancellationRequested)
        {
            db.Entry(reading).State = EntityState.Detached;
            var existing = await Readings(owner)
                .SingleOrDefaultAsync(x => x.ProcessVariableId == variable.Id && x.SourceKey == key, ct);

            if (existing is not null) return (existing, true);
            throw;
        }

        if (!reading.WithinRange)
            await alerts.RaiseAnomalyAlert(owner, variable.Id,
                $"Variable {variable.Id} is outside its configured range.", cancellationToken: ct);

        return (reading, false);
    }
}