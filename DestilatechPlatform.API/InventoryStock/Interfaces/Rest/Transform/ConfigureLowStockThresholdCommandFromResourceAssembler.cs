using DestilatechPlatform.API.InventoryStock.Domain.Model.Commands;
using DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Transform;

public static class ConfigureLowStockThresholdCommandFromResourceAssembler
{
    public static ConfigureLowStockThresholdCommand ToCommandFromResource(ConfigureLowStockThresholdResource resource,
        int stockItemId, int userId)
    {
        return new ConfigureLowStockThresholdCommand(userId, stockItemId, resource.LowStockThreshold);
    }
}
