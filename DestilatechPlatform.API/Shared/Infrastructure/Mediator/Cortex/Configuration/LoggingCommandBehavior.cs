using Cortex.Mediator.Commands;

namespace DestilatechPlatform.API.Shared.Infrastructure.Mediator.Cortex.Configuration;

public class LoggingCommandBehavior<TCommand>(
    ILogger<LoggingCommandBehavior<TCommand>> logger)
    : ICommandPipelineBehavior<TCommand> where TCommand : ICommand
{
    public async Task Handle(
        TCommand command,
        CommandHandlerDelegate next,
        CancellationToken ct)
    {
        var commandName = typeof(TCommand).Name;
        logger.LogInformation("Executing command {CommandName}", commandName);
        await next();
        logger.LogInformation("Executed command {CommandName}", commandName);
    }
}