namespace DestilatechPlatform.API.AlertsNotifications.Interfaces.Rest.Resources;

public record AlertResource(
    int Id,
    int TenantId,
    string Type,
    int SourceId,
    string Message,
    string Severity,
    string Status,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? AttendedAt);