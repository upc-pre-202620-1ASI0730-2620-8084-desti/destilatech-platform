using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

namespace DestilatechPlatform.API.AnalyticsEstimations.Interfaces.Rest.Transform;

/// <summary>
/// Converts enum values to the text the web app uses, for example STOCK_EXITS.
/// </summary>
public static class AnalyticsEstimationsValueAssembler
{
    public static string ToValueFromEnum(Enum member)
    {
        return member.ToString().ToSnakeCase().ToUpperInvariant();
    }
}
