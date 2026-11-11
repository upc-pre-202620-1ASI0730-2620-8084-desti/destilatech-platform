using DestilatechPlatform.API.Shared.Domain.Model.Errors;

namespace DestilatechPlatform.API.ProductionMonitoring.Domain.Model;

/// <summary>
/// Business error of the Production &amp; Monitoring bounded context.
/// </summary>
/// <param name="Code">Stable error code, in kebab case.</param>
/// <param name="Message">English description of the error.</param>
/// <param name="Type">Kind of error, used by the REST layer to choose the HTTP status code.</param>
public sealed record ProductionMonitoringError(string Code, string Message, EErrorType Type = EErrorType.Validation)
    : Error(Code, Message, Type)
{
    public const string BatchClosedCode = "batch-closed";
    public const string InvalidStageTransitionCode = "invalid-stage-transition";

    public static ProductionMonitoringError InvalidData(string message) =>
        new("invalid-data", message);

    public static ProductionMonitoringError BatchNotFound(int batchId) =>
        new("batch-not-found", $"Batch {batchId} was not found.", EErrorType.NotFound);

    public static ProductionMonitoringError ProcessVariableNotFound() =>
        new("process-variable-not-found", "The process variable was not found.", EErrorType.NotFound);

    public static ProductionMonitoringError BatchCodeAlreadyExists() =>
        new("batch-code-exists", "The batch code already exists.", EErrorType.Conflict);

    public static ProductionMonitoringError ProcessVariableAlreadyExists() =>
        new("process-variable-exists", "The variable already exists for this batch.", EErrorType.Conflict);

    public static ProductionMonitoringError BatchClosed() =>
        new(BatchClosedCode, "The batch is closed.", EErrorType.Conflict);

    public static ProductionMonitoringError InvalidStageTransition() =>
        new(InvalidStageTransitionCode, "Invalid batch stage transition.", EErrorType.Conflict);

    public static ProductionMonitoringError BottledVolumeExceedsEstimate() =>
        new("bottled-volume-exceeds-estimate", "Bottled volume exceeds the estimate. Confirm to continue.",
            EErrorType.Conflict);

    public static ProductionMonitoringError BottledProductRequired() =>
        new("bottled-product-required", "Select an active bottled product from your inventory.");

    public static ProductionMonitoringError MaximumStockExceeded() =>
        new("maximum-stock-exceeded", "Maximum stock exceeded.", EErrorType.Conflict);

    public static ProductionMonitoringError NotMonitoredStage() =>
        new("not-monitored-stage", "The batch is not in a monitored stage.", EErrorType.Conflict);

    public static ProductionMonitoringError BatchChanged() =>
        new("batch-changed", "Batch or stock changed. Refresh and try again.", EErrorType.Conflict);

    public static ProductionMonitoringError InvalidPeriod() =>
        new("invalid-period", "The start must be before the end.");

    public static ProductionMonitoringError SourceRequired() =>
        new("source-required", "Source ID and source key are required.");
}
