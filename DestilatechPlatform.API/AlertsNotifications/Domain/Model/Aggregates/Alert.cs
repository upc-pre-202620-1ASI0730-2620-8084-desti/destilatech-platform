using DestilatechPlatform.API.AlertsNotifications.Domain.Model.Commands;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model.ValueObjects;

namespace DestilatechPlatform.API.AlertsNotifications.Domain.Model.Aggregates;

public partial class Alert
{

    public int Id { get; private set; }
    
    public AccountId AccountId { get; private set; }

    public AlertType Type { get; private set; }

    public SourceId SourceId { get; private set; }


    public AlertMessage Message { get; private set; }

    public AlertSeverity Severity { get; private set; }


    public AlertStatus Status { get; private set; }


    public string? EpisodeKey { get; private set; }

    public DateTimeOffset? AttendedAt { get; private set; }

    public Alert()
    {
        AccountId = null!;
        SourceId = null!;
        Message = null!;
    }
    
    public Alert(RaiseAlertCommand command)
    {
        AccountId = command.AccountId;
        Type = command.Type;
        SourceId = command.SourceId;
        Message = command.Message;
        Severity = command.Severity;
        Status = AlertStatus.Pending;
        EpisodeKey = BuildEpisodeKey(command.AccountId, command.Type, command.SourceId);
    }


    public static string BuildEpisodeKey(AccountId accountId, AlertType type, SourceId sourceId) =>
        $"{type}:{accountId.Value}:{sourceId.Value}";
    
    public bool IsPending() => Status == AlertStatus.Pending;
    
    public void MarkAsAttended()
    {
        if (!IsPending())
            throw new InvalidOperationException("The alert was already attended.");
        Status = AlertStatus.Attended;
        AttendedAt = DateTimeOffset.UtcNow;
        EpisodeKey = null;
    }
}
