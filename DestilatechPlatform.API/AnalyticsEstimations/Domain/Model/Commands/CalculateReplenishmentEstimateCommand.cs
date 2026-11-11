namespace DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Commands;

/// <summary>
/// Calculates and stores a new replenishment estimate for one product (US20, US29).
/// </summary>
public record CalculateReplenishmentEstimateCommand(int UserId, int ProductId);
