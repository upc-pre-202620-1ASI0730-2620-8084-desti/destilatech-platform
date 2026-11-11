using DestilatechPlatform.API.InventoryStock.Interfaces.ACL;
using DestilatechPlatform.API.ProductionMonitoring.Application.Services;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Aggregates;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Commands;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Repositories;
using DestilatechPlatform.API.Shared.Application.Model;
using DestilatechPlatform.API.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.ProductionMonitoring.Application.Internal.CommandServices;

public class ProductionBatchCommandService(
    IProductionBatchRepository productionBatchRepository,
    IUnitOfWork unitOfWork,
    IInventoryContextFacade inventoryContextFacade) : IProductionBatchCommandService
{
    private const string BottledStage = "BOTTLED";

    public async Task<Result<ProductionBatch>> Handle(CreateProductionBatchCommand command,
        CancellationToken cancellationToken = default)
    {
        ProductionBatch batch;
        try
        {
            batch = new ProductionBatch(command.UserId, command.Code, command.Variety, command.StartDate,
                command.EstimatedQuantity, command.Tank, command.Notes);
        }
        catch (ArgumentException exception)
        {
            return Result<ProductionBatch>.Failure(ProductionMonitoringError.InvalidData(exception.Message));
        }

        if (await productionBatchRepository.ExistsByCodeAndUserIdAsync(batch.Code, command.UserId, cancellationToken))
            return Result<ProductionBatch>.Failure(ProductionMonitoringError.BatchCodeAlreadyExists());

        await productionBatchRepository.AddAsync(batch, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<ProductionBatch>.Success(batch);
    }

    public Task<Result<ProductionBatch>> Handle(UpdateProductionBatchCommand command,
        CancellationToken cancellationToken = default)
    {
        return ChangeBatchAsync(command.UserId, command.BatchId, command.Stage, true, command.Variety,
            command.EstimatedQuantity, command.Tank, command.Notes, command.ProductId, command.BottledUnits,
            command.StageNotes, command.ConfirmOverEstimate, command.BottleVolumeLiters, cancellationToken);
    }

    public Task<Result<ProductionBatch>> Handle(AdvanceBatchStageCommand command,
        CancellationToken cancellationToken = default)
    {
        return ChangeBatchAsync(command.UserId, command.BatchId, command.Stage, false, "", 0, null, null,
            command.ProductId, command.BottledUnits, command.Notes, command.ConfirmOverEstimate,
            command.BottleVolumeLiters, cancellationToken);
    }

    private async Task<Result<ProductionBatch>> ChangeBatchAsync(int userId, int batchId, string stage,
        bool updateDetails, string variety, decimal estimatedQuantity, string? tank, string? notes, int? productId,
        int bottledUnits, string? stageNotes, bool confirmOverEstimate, decimal bottleVolumeLiters,
        CancellationToken cancellationToken)
    {
        var batch = await productionBatchRepository.FindByIdAndUserIdAsync(batchId, userId, cancellationToken);
        if (batch is null)
            return Result<ProductionBatch>.Failure(ProductionMonitoringError.BatchNotFound(batchId));

        // Repeating the bottling with the same data is accepted, so a retried request does not fail.
        if (stage == batch.Stage && stage == BottledStage)
            return batch.ProductId == productId && batch.BottledUnits == bottledUnits
                ? Result<ProductionBatch>.Success(batch)
                : Result<ProductionBatch>.Failure(ProductionMonitoringError.BatchClosed());

        if (!updateDetails && stage == batch.Stage)
            return Result<ProductionBatch>.Failure(ProductionMonitoringError.InvalidStageTransition());

        try
        {
            if (updateDetails)
                batch.Edit(variety, estimatedQuantity, tank, notes, recordChange: stage == batch.Stage);

            if (batch.Stage != stage)
            {
                if (stage == BottledStage)
                {
                    if (bottleVolumeLiters <= 0 || bottleVolumeLiters > 10)
                        return Result<ProductionBatch>.Failure(
                            ProductionMonitoringError.InvalidData("Invalid volume per bottle."));

                    if (!confirmOverEstimate && bottledUnits * bottleVolumeLiters > batch.EstimatedQuantity)
                        return Result<ProductionBatch>.Failure(
                            ProductionMonitoringError.BottledVolumeExceedsEstimate());
                }

                batch.Advance(stage, productId, bottledUnits, stageNotes);

                if (stage == BottledStage && !await inventoryContextFacade.StageBottledStockAsync(userId,
                        batch.ProductId!.Value, bottledUnits, batch.Code, cancellationToken))
                    return Result<ProductionBatch>.Failure(ProductionMonitoringError.BottledProductRequired());
            }

            // The batch and the bottled stock entry are saved together.
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (ArgumentException exception)
        {
            return Result<ProductionBatch>.Failure(ProductionMonitoringError.InvalidData(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return Result<ProductionBatch>.Failure(exception.Message switch
            {
                "The batch is closed." => ProductionMonitoringError.BatchClosed(),
                "Maximum stock exceeded." => ProductionMonitoringError.MaximumStockExceeded(),
                _ => ProductionMonitoringError.InvalidStageTransition()
            });
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result<ProductionBatch>.Failure(ProductionMonitoringError.BatchChanged());
        }

        return Result<ProductionBatch>.Success(batch);
    }
}
