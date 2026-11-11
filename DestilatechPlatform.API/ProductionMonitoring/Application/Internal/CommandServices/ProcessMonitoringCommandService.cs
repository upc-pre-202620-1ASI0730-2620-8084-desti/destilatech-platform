using DestilatechPlatform.API.AlertsNotifications.Interfaces.ACL;
using DestilatechPlatform.API.ProductionMonitoring.Application.Services;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Aggregates;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Commands;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Entities;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.ValueObjects;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Repositories;
using DestilatechPlatform.API.Shared.Application.Model;
using DestilatechPlatform.API.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.ProductionMonitoring.Application.Internal.CommandServices;

public class ProcessMonitoringCommandService(
    IProductionBatchRepository productionBatchRepository,
    IProcessVariableRepository processVariableRepository,
    ISensorReadingRepository sensorReadingRepository,
    IUnitOfWork unitOfWork,
    IAlertsContextFacade alertsContextFacade) : IProcessMonitoringCommandService
{
    private static readonly string[] MonitoredStages = ["FERMENTATION", "DISTILLATION", "RESTING"];

    public async Task<Result<ProcessVariable>> Handle(AddProcessVariableCommand command,
        CancellationToken cancellationToken = default)
    {
        ProcessVariable variable;
        try
        {
            variable = new ProcessVariable(command.UserId, command.BatchId, command.Type, command.MinRange,
                command.MaxRange, command.SensorId);
        }
        catch (ArgumentException exception)
        {
            return Result<ProcessVariable>.Failure(ProductionMonitoringError.InvalidData(exception.Message));
        }

        if (!await productionBatchRepository.ExistsByIdAndUserIdAsync(command.BatchId, command.UserId,
                cancellationToken))
            return Result<ProcessVariable>.Failure(ProductionMonitoringError.BatchNotFound(command.BatchId));

        if (await processVariableRepository.ExistsByBatchIdAndTypeAsync(command.BatchId, variable.Type,
                command.UserId, cancellationToken))
            return Result<ProcessVariable>.Failure(ProductionMonitoringError.ProcessVariableAlreadyExists());

        await processVariableRepository.AddAsync(variable, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<ProcessVariable>.Success(variable);
    }

    public async Task<Result<ProcessVariable>> Handle(ConfigureVariableRangeCommand command,
        CancellationToken cancellationToken = default)
    {
        var variable = await processVariableRepository.FindByIdAndUserIdAsync(command.ProcessVariableId,
            command.UserId, cancellationToken);
        if (variable is null)
            return Result<ProcessVariable>.Failure(ProductionMonitoringError.ProcessVariableNotFound());

        try
        {
            variable.ConfigureRange(command.MinRange, command.MaxRange);
        }
        catch (ArgumentException exception)
        {
            return Result<ProcessVariable>.Failure(ProductionMonitoringError.InvalidData(exception.Message));
        }

        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<ProcessVariable>.Success(variable);
    }

    public async Task<Result<RecordedSensorReading>> Handle(RecordSensorReadingCommand command,
        CancellationToken cancellationToken = default)
    {
        var variable = await processVariableRepository.FindForReadingAsync(command.UserId, command.ProcessVariableId,
            command.BatchId, command.SourceId, cancellationToken);
        if (variable is null)
            return Result<RecordedSensorReading>.Failure(ProductionMonitoringError.ProcessVariableNotFound());

        var sourceKey = string.IsNullOrWhiteSpace(command.SourceKey) ? null : command.SourceKey.Trim();
        if (sourceKey?.Length > 80)
            return Result<RecordedSensorReading>.Failure(
                ProductionMonitoringError.InvalidData("Source key is too long."));

        if (sourceKey is not null)
        {
            var existing = await sensorReadingRepository.FindBySourceKeyAsync(command.UserId, variable.Id, sourceKey,
                cancellationToken);
            if (existing is not null)
                return Result<RecordedSensorReading>.Success(new RecordedSensorReading(existing, true));
        }

        var batch = await productionBatchRepository.FindByIdAndUserIdAsync(variable.BatchId, command.UserId,
            cancellationToken);
        if (batch is null)
            return Result<RecordedSensorReading>.Failure(ProductionMonitoringError.BatchNotFound(variable.BatchId));

        if (!MonitoredStages.Contains(batch.Stage))
            return Result<RecordedSensorReading>.Failure(ProductionMonitoringError.NotMonitoredStage());

        SensorReading reading;
        try
        {
            reading = new SensorReading(variable, command.Value, command.RecordedAt, sourceKey);
        }
        catch (ArgumentException exception)
        {
            return Result<RecordedSensorReading>.Failure(ProductionMonitoringError.InvalidData(exception.Message));
        }

        try
        {
            await sensorReadingRepository.AddAsync(reading, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (DbUpdateException) when (sourceKey is not null && !cancellationToken.IsCancellationRequested)
        {
            // Another request recorded the same source key at the same time: return that reading.
            var existing = await sensorReadingRepository.FindBySourceKeyAsync(command.UserId, variable.Id, sourceKey,
                cancellationToken);
            if (existing is not null)
                return Result<RecordedSensorReading>.Success(new RecordedSensorReading(existing, true));
            throw;
        }

        if (!reading.WithinRange)
            await alertsContextFacade.RaiseAnomalyAlert(command.UserId, variable.Id,
                $"Variable {variable.Id} is outside its configured range.", cancellationToken: cancellationToken);

        return Result<RecordedSensorReading>.Success(new RecordedSensorReading(reading, false));
    }
}
