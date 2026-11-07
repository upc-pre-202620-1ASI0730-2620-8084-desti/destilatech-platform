using DestilatechPlatform.API.AnalyticsEstimations.Application.Internal.OutboundServices;
using DestilatechPlatform.API.AnalyticsEstimations.Application.Services;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Aggregates;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Commands;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Repositories;
using DestilatechPlatform.API.Shared.Application.Patterns;
using DestilatechPlatform.API.Shared.Domain.Repositories;

namespace DestilatechPlatform.API.AnalyticsEstimations.Application.Internal.CommandServices;

/// <summary>
/// Calculates replenishment estimates from the inventory history and stores them (US20, US29).
/// A product without enough history still gets an estimate, with <c>HasEnoughData</c> set to false.
/// </summary>
public class ReplenishmentEstimateCommandService(
    IReplenishmentEstimateRepository estimateRepository,
    IInventoryHistoryReadPort inventoryHistory,
    IUnitOfWork unitOfWork) : IReplenishmentEstimateCommandService
{
    public async Task<Result<ReplenishmentEstimate, AnalyticsEstimationsError>> HandleAsync(
        CalculateReplenishmentEstimateCommand command, CancellationToken cancellationToken = default)
    {
        var stockLevel = await inventoryHistory.ReadStockLevelAsync(command.UserId, command.ProductId,
            cancellationToken);
        if (stockLevel is null)
        {
            return new Result<ReplenishmentEstimate, AnalyticsEstimationsError>.Failure(
                AnalyticsEstimationsError.ProductNotFound(command.ProductId));
        }

        var calculatedAt = DateTimeOffset.UtcNow;
        var movements = await inventoryHistory.ReadProductMovementsAsync(command.UserId, command.ProductId,
            WindowStart(calculatedAt), cancellationToken);

        ReplenishmentEstimate estimate;
        try
        {
            estimate = ReplenishmentEstimate.Calculate(command.UserId, stockLevel.Value, movements, calculatedAt);
        }
        catch (AnalyticsEstimationsException exception)
        {
            return new Result<ReplenishmentEstimate, AnalyticsEstimationsError>.Failure(exception.Error);
        }

        await estimateRepository.AddAsync(estimate, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<ReplenishmentEstimate, AnalyticsEstimationsError>.Success(estimate);
    }

    public async Task<Result<IReadOnlyCollection<ReplenishmentEstimate>, AnalyticsEstimationsError>> HandleAsync(
        RecalculateReplenishmentEstimatesCommand command, CancellationToken cancellationToken = default)
    {
        var calculatedAt = DateTimeOffset.UtcNow;
        var stockLevels = await inventoryHistory.ReadStockLevelsAsync(command.UserId, cancellationToken);
        var movements = await inventoryHistory.ReadMovementsAsync(command.UserId, WindowStart(calculatedAt),
            cancellationToken);

        List<ReplenishmentEstimate> estimates;
        try
        {
            estimates = stockLevels
                .Select(stockLevel =>
                    ReplenishmentEstimate.Calculate(command.UserId, stockLevel, movements, calculatedAt))
                .ToList();
        }
        catch (AnalyticsEstimationsException exception)
        {
            return new Result<IReadOnlyCollection<ReplenishmentEstimate>, AnalyticsEstimationsError>.Failure(
                exception.Error);
        }

        foreach (var estimate in estimates)
        {
            await estimateRepository.AddAsync(estimate, cancellationToken);
        }

        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<IReadOnlyCollection<ReplenishmentEstimate>, AnalyticsEstimationsError>.Success(estimates);
    }

    private static DateTimeOffset WindowStart(DateTimeOffset calculatedAt)
    {
        return calculatedAt.AddDays(-ReplenishmentEstimate.AnalysisWindowDays);
    }
}
