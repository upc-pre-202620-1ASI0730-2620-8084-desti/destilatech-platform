using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Aggregates;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Commands;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.ValueObjects;
using DestilatechPlatform.API.Shared.Application.Model;

namespace DestilatechPlatform.API.ProductionMonitoring.Application.Services;

public interface IProcessMonitoringCommandService
{
    Task<Result<ProcessVariable>> Handle(AddProcessVariableCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<ProcessVariable>> Handle(ConfigureVariableRangeCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<RecordedSensorReading>> Handle(RecordSensorReadingCommand command,
        CancellationToken cancellationToken = default);
}
