using System.ComponentModel.DataAnnotations;

namespace DestilatechPlatform.API.AlertsNotifications.Interfaces.Rest.Resources;

public record RaiseAlertResource(
    [Range(1, int.MaxValue, ErrorMessage = "InvalidAlertAccountId")]
    int TenantId,
    [Required(ErrorMessage = "RequiredAlertType")]
    string Type,
    [Range(1, int.MaxValue, ErrorMessage = "InvalidAlertSourceId")]
    int SourceId,
    [Required(ErrorMessage = "RequiredAlertMessage")]
    [StringLength(255, ErrorMessage = "InvalidAlertMessageLength")]
    string Message,
    string? Severity = null);