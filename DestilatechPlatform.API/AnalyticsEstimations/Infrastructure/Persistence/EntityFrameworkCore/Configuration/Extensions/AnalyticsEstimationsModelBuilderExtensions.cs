using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.AnalyticsEstimations.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

/// <summary>
/// Entity Framework Core mapping of the Analytics &amp; Estimations bounded context.
/// Each calculated estimate is kept in the <c>replenishment_estimates</c> table, so the latest one
/// of a product can be shown without recalculating it.
/// </summary>
public static class AnalyticsEstimationsModelBuilderExtensions
{
    // Same precision as the stock quantities of Inventory & Stock Management.
    private const int QuantityPrecision = 12;
    private const int QuantityScale = 3;
    private const int ConsumptionScale = 2;
    private const int ConfidencePrecision = 3;
    private const int ConfidenceScale = 2;

    public static void ApplyAnalyticsEstimationsConfiguration(this ModelBuilder builder)
    {
        builder.ApplyReplenishmentEstimateConfiguration();
    }

    private static void ApplyReplenishmentEstimateConfiguration(this ModelBuilder builder)
    {
        builder.Entity<ReplenishmentEstimate>().HasKey(e => e.Id);
        builder.Entity<ReplenishmentEstimate>().Property(e => e.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<ReplenishmentEstimate>().Property(e => e.UserId).IsRequired();
        builder.Entity<ReplenishmentEstimate>().Property(e => e.ProductId).IsRequired();
        builder.Entity<ReplenishmentEstimate>().Property(e => e.HasEnoughData).IsRequired();
        builder.Entity<ReplenishmentEstimate>().Property(e => e.AverageDailyConsumption)
            .IsRequired()
            .HasPrecision(QuantityPrecision, ConsumptionScale);
        builder.Entity<ReplenishmentEstimate>().Property(e => e.DaysUntilThreshold);
        builder.Entity<ReplenishmentEstimate>().Property(e => e.EstimatedDate);
        builder.Entity<ReplenishmentEstimate>().Property(e => e.SuggestedQuantity).IsRequired();
        builder.Entity<ReplenishmentEstimate>().Property(e => e.Confidence)
            .IsRequired()
            .HasPrecision(ConfidencePrecision, ConfidenceScale);
        builder.Entity<ReplenishmentEstimate>().Property(e => e.ExitsAnalyzed).IsRequired();
        builder.Entity<ReplenishmentEstimate>().Property(e => e.CurrentQuantitySnapshot)
            .IsRequired()
            .HasPrecision(QuantityPrecision, QuantityScale);
        builder.Entity<ReplenishmentEstimate>().Property(e => e.LowStockThresholdSnapshot)
            .IsRequired()
            .HasPrecision(QuantityPrecision, QuantityScale);
        builder.Entity<ReplenishmentEstimate>().Property(e => e.CalculatedAt).IsRequired();

        // The latest estimate of each product is searched by account and product.
        builder.Entity<ReplenishmentEstimate>().HasIndex(e => new { e.UserId, e.ProductId });
    }
}
