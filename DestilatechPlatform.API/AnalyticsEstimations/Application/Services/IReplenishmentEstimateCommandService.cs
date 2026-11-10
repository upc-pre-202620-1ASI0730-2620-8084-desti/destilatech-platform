using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Aggregates;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Commands;
using DestilatechPlatform.API.Shared.Application.Model;

namespace DestilatechPlatform.API.AnalyticsEstimations.Application.Services;

public interface IReplenishmentEstimateCommandService
{
    Task<Result<ReplenishmentEstimate>> Handle(
        CalculateReplenishmentEstimateCommand command, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyCollection<ReplenishmentEstimate>>> Handle(
        RecalculateReplenishmentEstimatesCommand command, CancellationToken cancellationToken = default);
}
