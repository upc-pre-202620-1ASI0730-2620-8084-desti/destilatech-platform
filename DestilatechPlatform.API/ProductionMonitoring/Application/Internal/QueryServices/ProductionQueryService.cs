using DestilatechPlatform.API.ProductionMonitoring.Application.Services;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Aggregates;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Entities;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Queries;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Repositories;
using DestilatechPlatform.API.Shared.Application.Model;

namespace DestilatechPlatform.API.ProductionMonitoring.Application.Internal.QueryServices;

public class ProductionQueryService(
    IProductionBatchRepository productionBatchRepository,
    IProcessVariableRepository processVariableRepository,
    ISensorReadingRepository sensorReadingRepository) : IProductionQueryService
{
    private const int MaxLatestReadings = 500;

    public async Task<IEnumerable<ProductionBatch>> Handle(GetAllProductionBatchesByUserIdQuery query,
        CancellationToken cancellationToken = default)
    {
        return await productionBatchRepository.FindByUserIdAsync(query.UserId, cancellationToken);
    }

    public async Task<IEnumerable<ProcessVariable>> Handle(GetAllProcessVariablesByUserIdQuery query,
        CancellationToken cancellationToken = default)
    {
        return await processVariableRepository.FindByUserIdAsync(query.UserId, cancellationToken);
    }

    public async Task<IEnumerable<SensorReading>> Handle(GetLatestSensorReadingsByUserIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var limit = Math.Clamp(query.Limit ?? MaxLatestReadings, 1, MaxLatestReadings);
        return await sensorReadingRepository.FindLatestByUserIdAsync(query.UserId, limit, cancellationToken);
    }

    public async Task<Result<IEnumerable<SensorReading>>> Handle(GetBatchSensorReadingsQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.From is not null && query.To is not null && query.From > query.To)
            return Result<IEnumerable<SensorReading>>.Failure(ProductionMonitoringError.InvalidPeriod());

        if (!await productionBatchRepository.ExistsByIdAndUserIdAsync(query.BatchId, query.UserId, cancellationToken))
            return Result<IEnumerable<SensorReading>>.Failure(ProductionMonitoringError.BatchNotFound(query.BatchId));

        string? variableType = null;
        if (!string.IsNullOrWhiteSpace(query.VariableType))
        {
            variableType = query.VariableType.Trim().ToUpperInvariant();
            if (variableType == "TEMPERATURA") variableType = "TEMPERATURE";
        }

        var readings = await sensorReadingRepository.FindByBatchIdAsync(query.UserId, query.BatchId, variableType,
            query.From?.UtcDateTime, query.To?.UtcDateTime, cancellationToken);
        return Result<IEnumerable<SensorReading>>.Success(readings);
    }
}
