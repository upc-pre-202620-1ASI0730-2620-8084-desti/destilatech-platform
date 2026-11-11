using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.ValueObjects;
using DestilatechPlatform.API.AnalyticsEstimations.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.AnalyticsEstimations.Interfaces.Rest.Transform;

public static class ProductConsumptionResourceFromValueAssembler
{
    public static ProductConsumptionResource ToResourceFromValue(ProductConsumption value)
    {
        return new ProductConsumptionResource(value.ProductId, value.Units);
    }
}
