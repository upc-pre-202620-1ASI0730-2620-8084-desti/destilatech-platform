using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Commands;
using DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Transform;

public static class CreateProductionBatchCommandFromResourceAssembler
{
    public static CreateProductionBatchCommand ToCommandFromResource(CreateProductionBatchResource resource,
        int userId)
    {
        return new CreateProductionBatchCommand(userId, resource.Code, resource.Variety, resource.StartDate,
            resource.EstimatedQuantity, resource.Tank, resource.Notes);
    }
}
