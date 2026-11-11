using DestilatechPlatform.API.InventoryStock.Domain.Model.Aggregates;
using DestilatechPlatform.API.InventoryStock.Domain.Model.Commands;
using DestilatechPlatform.API.InventoryStock.Domain.Model.Entities;
using DestilatechPlatform.API.Shared.Application.Model;

namespace DestilatechPlatform.API.InventoryStock.Application.Services;

public interface IStockItemCommandService
{
    Task<Result<StockItem>> Handle(ConfigureLowStockThresholdCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<StockMovement>> Handle(RegisterStockMovementCommand command,
        CancellationToken cancellationToken = default);
}
