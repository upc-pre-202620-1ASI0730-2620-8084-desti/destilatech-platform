using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Aggregates;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Entities;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Queries;
using DestilatechPlatform.API.Shared.Application.Model;

namespace DestilatechPlatform.API.ProductionMonitoring.Application.Services;

public interface IProductionQueryService
{
    Task<IEnumerable<ProductionBatch>> Handle(GetAllProductionBatchesByUserIdQuery query,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<ProcessVariable>> Handle(GetAllProcessVariablesByUserIdQuery query,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<SensorReading>> Handle(GetLatestSensorReadingsByUserIdQuery query,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<SensorReading>>> Handle(GetBatchSensorReadingsQuery query,
        CancellationToken cancellationToken = default);
}
