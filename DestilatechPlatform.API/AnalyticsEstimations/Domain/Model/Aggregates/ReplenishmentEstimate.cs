using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.ValueObjects;
using DestilatechPlatform.API.Shared.Domain.Model.Aggregates.Internal;

namespace DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Aggregates;

/// <summary>
/// Estimate of when a product will reach its low stock threshold and how many units to reorder
/// (US20, US29). Each calculation is stored with the stock data it used, so past results can be reviewed.
/// </summary>
/// <remarks>
/// The calculation follows the web application: it looks at the stock exits caused by sales, orders
/// and waste in the last <see cref="AnalysisWindowDays"/> days and needs at least
/// <see cref="MinExitMovements"/> of them to give an estimate.
/// </remarks>
public partial class ReplenishmentEstimate : AggregateRoot
{
    public const int AnalysisWindowDays = 30;
    public const int CoverageDays = 30;
    public const int MinExitMovements = 3;
    public const int UrgentWithinDays = 7;

    private const int ExitsForFullConfidence = 10;
    private const int ConsumptionDecimals = 2;

    private static readonly string[] ConsumptionReasons = ["SALE", "ORDER", "WASTE"];

    public int Id { get; private set; }

    public int UserId { get; private set; }

    public int ProductId { get; private set; }

    /// <summary>Whether there were enough stock exits in the window to estimate.</summary>
    public bool HasEnoughData { get; private set; }

    /// <summary>Average units consumed per day in the window, rounded to 2 decimals.</summary>
    public decimal AverageDailyConsumption { get; private set; }

    /// <summary>Days until the stock reaches its low stock threshold. 0 means it already did.</summary>
    public int? DaysUntilThreshold { get; private set; }

    /// <summary>Date on which the stock is expected to reach its low stock threshold.</summary>
    public DateTimeOffset? EstimatedDate { get; private set; }

    /// <summary>Units to reorder to cover the next <see cref="CoverageDays"/> days above the threshold.</summary>
    public int SuggestedQuantity { get; private set; }

    /// <summary>From 0 to 1. Grows with the number of exits analyzed and reaches 1 with ten exits.</summary>
    public decimal Confidence { get; private set; }

    public int ExitsAnalyzed { get; private set; }

    public decimal CurrentQuantitySnapshot { get; private set; }

    public decimal LowStockThresholdSnapshot { get; private set; }

    public DateTimeOffset CalculatedAt { get; private set; }

    /// <summary>
    /// Constructor required by Entity Framework Core.
    /// </summary>
    protected ReplenishmentEstimate()
    {
    }

    private ReplenishmentEstimate(int userId, StockLevel stockLevel, int exitsAnalyzed, DateTimeOffset calculatedAt)
    {
        UserId = userId;
        ProductId = stockLevel.ProductId;
        CurrentQuantitySnapshot = stockLevel.CurrentQuantity;
        LowStockThresholdSnapshot = stockLevel.LowStockThreshold;
        ExitsAnalyzed = exitsAnalyzed;
        CalculatedAt = calculatedAt;
    }

    /// <summary>
    /// Calculates the estimate of a product from its stock level and its stock movements.
    /// </summary>
    /// <param name="userId">Account that owns the product.</param>
    /// <param name="stockLevel">Current stock and low stock threshold of the product.</param>
    /// <param name="movements">Stock movements of the account. Only the consumption exits of the product
    /// in the analysis window are used.</param>
    /// <param name="calculatedAt">Moment of the calculation.</param>
    public static ReplenishmentEstimate Calculate(int userId, StockLevel stockLevel,
        IEnumerable<StockMovementRecord> movements, DateTimeOffset calculatedAt)
    {
        if (userId <= 0 || stockLevel.ProductId <= 0)
        {
            throw new AnalyticsEstimationsException(
                AnalyticsEstimationsError.InvalidData("An estimate needs an account and a product."));
        }

        var windowStart = calculatedAt.AddDays(-AnalysisWindowDays);
        var exits = (movements ?? [])
            .Where(movement => movement.ProductId == stockLevel.ProductId
                               && movement.Direction == MovementDirection.Out
                               && IsConsumption(movement.Reason)
                               && movement.Date >= windowStart
                               && movement.Date <= calculatedAt)
            .ToList();

        var estimate = new ReplenishmentEstimate(userId, stockLevel, exits.Count, calculatedAt);
        if (exits.Count < MinExitMovements)
        {
            estimate.HasEnoughData = false;
            return estimate;
        }

        decimal consumed = exits.Sum(exit => exit.Quantity);
        var averageDailyConsumption = consumed / AnalysisWindowDays;
        var unitsAboveThreshold = stockLevel.CurrentQuantity - stockLevel.LowStockThreshold;
        var daysUntilThreshold = unitsAboveThreshold <= 0
            ? 0
            : (int)Math.Floor(unitsAboveThreshold * AnalysisWindowDays / consumed);
        // Multiply before dividing, so a whole result is not pushed up by a rounded division.
        var coverageConsumption = consumed * CoverageDays / AnalysisWindowDays;
        var suggestedQuantity = Math.Ceiling(coverageConsumption
                                             + stockLevel.LowStockThreshold - stockLevel.CurrentQuantity);

        estimate.HasEnoughData = true;
        estimate.AverageDailyConsumption =
            Math.Round(averageDailyConsumption, ConsumptionDecimals, MidpointRounding.AwayFromZero);
        estimate.DaysUntilThreshold = daysUntilThreshold;
        estimate.EstimatedDate = calculatedAt.AddDays(daysUntilThreshold);
        estimate.SuggestedQuantity = (int)Math.Max(0, suggestedQuantity);
        estimate.Confidence = Math.Min(1m, (decimal)exits.Count / ExitsForFullConfidence);
        return estimate;
    }

    /// <summary>
    /// Whether the product reaches its low stock threshold within <see cref="UrgentWithinDays"/> days.
    /// </summary>
    public bool IsUrgent()
    {
        return HasEnoughData && DaysUntilThreshold <= UrgentWithinDays;
    }

    public bool BelongsTo(int userId)
    {
        return UserId == userId;
    }

    private static bool IsConsumption(string reason)
    {
        return ConsumptionReasons.Contains(reason?.Trim(), StringComparer.OrdinalIgnoreCase);
    }
}
