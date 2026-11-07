using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Aggregates;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Commands;
using DestilatechPlatform.API.Shared.Application.Patterns;

namespace DestilatechPlatform.API.AnalyticsEstimations.Application.Services;

public interface IReplenishmentEstimateCommandService
{
    Task<Result<ReplenishmentEstimate, AnalyticsEstimationsError>> HandleAsync(
        CalculateReplenishmentEstimateCommand command, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyCollection<ReplenishmentEstimate>, AnalyticsEstimationsError>> HandleAsync(
        RecalculateReplenishmentEstimatesCommand command, CancellationToken cancellationToken = default);
}
