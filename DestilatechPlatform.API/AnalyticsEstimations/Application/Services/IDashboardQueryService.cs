using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Queries;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.ValueObjects;
using DestilatechPlatform.API.Shared.Application.Model;

namespace DestilatechPlatform.API.AnalyticsEstimations.Application.Services;

public interface IDashboardQueryService
{
    Task<Result<DashboardSummary>> Handle(GetDashboardSummaryQuery query, CancellationToken cancellationToken = default);
}
