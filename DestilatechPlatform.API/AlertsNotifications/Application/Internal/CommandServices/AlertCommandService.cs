using DestilatechPlatform.API.AlertsNotifications.Application.Services;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model.Aggregates;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model.Commands;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model.Errors;
using DestilatechPlatform.API.AlertsNotifications.Domain.Repositories;
using DestilatechPlatform.API.Shared.Application.Model;
using DestilatechPlatform.API.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.AlertsNotifications.Application.Internal.CommandServices;

/// <summary>
///     Handles the commands of the <see cref="Alert" /> aggregate.
/// </summary>
/// <remarks>
///     Expected failures are returned as <see cref="Result{T}" />. Unexpected exceptions are left to
///     the global exception handler.
/// </remarks>
public class AlertCommandService(
    IAlertRepository alertRepository,
    IUnitOfWork unitOfWork,
    ILogger<AlertCommandService> logger) : IAlertCommandService
{
    private static bool IsDuplicateKeyViolation(DbUpdateException exception)
    {
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            if (!string.Equals(current.GetType().Name, "MySqlException", StringComparison.Ordinal)) continue;
            var numberProperty = current.GetType().GetProperty("Number");
            if (numberProperty?.PropertyType == typeof(int) &&
                numberProperty.GetValue(current) is int errorCode && errorCode == 1062) return true;
        }
        return false;
    }

    /// <inheritdoc />
    public async Task<Result<Alert>> Handle(RaiseAlertCommand command, CancellationToken cancellationToken)
    {
        // Policy AvoidDuplicateEpisode: one pending alert per account, type and source.
        var episodeKey = Alert.BuildEpisodeKey(command.AccountId, command.Type, command.SourceId);
        var pendingAlert = await alertRepository.FindPendingByEpisodeKeyAsync(episodeKey, cancellationToken);
        if (pendingAlert is not null)
        {
            logger.LogInformation("Duplicate pending alert rejected for episode {EpisodeKey}", episodeKey);
            return Result<Alert>.Failure(AlertErrors.DuplicatePendingAlert, AlertsError.DuplicatePendingAlert);
        }

        try
        {
            var alert = new Alert(command);
            await alertRepository.AddAsync(alert, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Alert>.Success(alert);
        }
        catch (DbUpdateException ex) when (IsDuplicateKeyViolation(ex))
        {
            logger.LogWarning(ex, "Duplicate key violation raising alert for episode {EpisodeKey}", episodeKey);
            return Result<Alert>.Failure(AlertErrors.DuplicatePendingAlert, AlertsError.DuplicatePendingAlert);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database update failed raising alert for episode {EpisodeKey}", episodeKey);
            return Result<Alert>.Failure(AlertErrors.DatabaseError, AlertsError.DatabaseError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Alert>> Handle(MarkAlertAsAttendedCommand command, CancellationToken cancellationToken)
    {
        var alert = await alertRepository.FindByIdAsync(command.AlertId, cancellationToken);
        if (alert is null)
            return Result<Alert>.Failure(AlertErrors.AlertNotFound, AlertsError.AlertNotFound);
        if (!alert.IsPending())
            return Result<Alert>.Failure(AlertErrors.AlertAlreadyAttended, AlertsError.AlertAlreadyAttended);

        try
        {
            alert.MarkAsAttended();
            alertRepository.Update(alert);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Alert>.Success(alert);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database update failed attending alert {AlertId}", command.AlertId);
            return Result<Alert>.Failure(AlertErrors.DatabaseError, AlertsError.DatabaseError);
        }
    }
}
