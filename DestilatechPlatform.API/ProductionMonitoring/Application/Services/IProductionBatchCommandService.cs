using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Aggregates;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Commands;
using DestilatechPlatform.API.Shared.Application.Model;

namespace DestilatechPlatform.API.ProductionMonitoring.Application.Services;

public interface IProductionBatchCommandService
{
    Task<Result<ProductionBatch>> Handle(CreateProductionBatchCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<ProductionBatch>> Handle(UpdateProductionBatchCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<ProductionBatch>> Handle(AdvanceBatchStageCommand command,
        CancellationToken cancellationToken = default);
}
