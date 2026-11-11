using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

namespace DestilatechPlatform.API.AlertsNotifications.Interfaces.Rest.Transform;

public static class AlertValueAssembler
{

    public static TEnum ToEnumFromValue<TEnum>(string value) where TEnum : struct, Enum
    {
        var normalized = value?.Replace("_", string.Empty).Trim();
        if (string.IsNullOrEmpty(normalized) || int.TryParse(normalized, out _) ||
            !Enum.TryParse<TEnum>(normalized, ignoreCase: true, out var member))
            throw new ArgumentException($"'{value}' is not a valid {typeof(TEnum).Name}.", nameof(value));
        return member;
    }

    public static string ToValueFromEnum(Enum member) => member.ToString().ToSnakeCase().ToUpperInvariant();
}