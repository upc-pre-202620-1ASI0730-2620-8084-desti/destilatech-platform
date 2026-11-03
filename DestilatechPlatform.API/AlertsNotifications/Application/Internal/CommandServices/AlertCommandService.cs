using DestilatechPlatform.API.AlertsNotifications.Application.Errors;
using DestilatechPlatform.API.AlertsNotifications.Application.Services;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model.Aggregates;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model.Commands;
using DestilatechPlatform.API.AlertsNotifications.Domain.Repositories;
using DestilatechPlatform.API.Shared.Application.Patterns;
using DestilatechPlatform.API.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.AlertsNotifications.Application.Internal.CommandServices;

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

    public async Task<Result<Alert, ERaiseAlertError>> Handle(RaiseAlertCommand command,
        CancellationToken cancellationToken)
    {
        var episodeKey = Alert.BuildEpisodeKey(command.AccountId, command.Type, command.SourceId);
        var pendingAlert = await alertRepository.FindPendingByEpisodeKeyAsync(episodeKey, cancellationToken);
        if (pendingAlert is not null)
        {
            logger.LogInformation("Duplicate pending alert rejected for episode {EpisodeKey}", episodeKey);
            return new Result<Alert, ERaiseAlertError>.Failure(ERaiseAlertError.DuplicatePendingAlert);
        }

        try
        {
            var alert = new Alert(command);
            await alertRepository.AddAsync(alert, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<Alert, ERaiseAlertError>.Success(alert);
        }
        catch (DbUpdateException ex) when (IsDuplicateKeyViolation(ex))
        {
            logger.LogWarning(ex, "Duplicate key violation raising alert for episode {EpisodeKey}", episodeKey);
            return new Result<Alert, ERaiseAlertError>.Failure(ERaiseAlertError.DuplicatePendingAlert);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database update failed raising alert for episode {EpisodeKey}", episodeKey);
            return new Result<Alert, ERaiseAlertError>.Failure(ERaiseAlertError.UnexpectedError);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error raising alert for episode {EpisodeKey}", episodeKey);
            return new Result<Alert, ERaiseAlertError>.Failure(ERaiseAlertError.UnexpectedError);
        }
    }

    public async Task<Result<Alert, EMarkAlertAsAttendedError>> Handle(MarkAlertAsAttendedCommand command,
        CancellationToken cancellationToken)
    {
        var alert = await alertRepository.FindByIdAsync(command.AlertId, cancellationToken);
        if (alert is null)
            return new Result<Alert, EMarkAlertAsAttendedError>.Failure(EMarkAlertAsAttendedError.AlertNotFound);
        if (!alert.IsPending())
            return new Result<Alert, EMarkAlertAsAttendedError>.Failure(
                EMarkAlertAsAttendedError.AlertAlreadyAttended);

        try
        {
            alert.MarkAsAttended();
            alertRepository.Update(alert);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<Alert, EMarkAlertAsAttendedError>.Success(alert);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database update failed attending alert {AlertId}", command.AlertId);
            return new Result<Alert, EMarkAlertAsAttendedError>.Failure(EMarkAlertAsAttendedError.UnexpectedError);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error attending alert {AlertId}", command.AlertId);
            return new Result<Alert, EMarkAlertAsAttendedError>.Failure(EMarkAlertAsAttendedError.UnexpectedError);
        }
    }
}
