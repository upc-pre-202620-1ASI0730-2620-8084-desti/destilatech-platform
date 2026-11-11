using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Aggregates;
using DestilatechPlatform.API.AnalyticsEstimations.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.AnalyticsEstimations.Interfaces.Rest.Transform;

public static class ReplenishmentEstimateResourceFromEntityAssembler
{
    public static ReplenishmentEstimateResource ToResourceFromEntity(ReplenishmentEstimate entity)
    {
        return new ReplenishmentEstimateResource(
            entity.Id,
            entity.UserId,
            entity.ProductId,
            entity.HasEnoughData,
            entity.AverageDailyConsumption,
            entity.DaysUntilThreshold,
            entity.EstimatedDate,
            entity.SuggestedQuantity,
            entity.Confidence,
            entity.ExitsAnalyzed,
            entity.CurrentQuantitySnapshot,
            entity.LowStockThresholdSnapshot,
            entity.IsUrgent(),
            entity.CalculatedAt);
    }
}
