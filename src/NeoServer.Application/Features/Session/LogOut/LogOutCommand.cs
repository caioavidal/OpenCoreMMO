using Mediator;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Application.Features.Session.LogOut;

public record LogOutCommand(IPlayer Player, bool Force) : ICommand;

public class LogOutCommandHandler : ICommandHandler<LogOutCommand>
{
    public ValueTask<Unit> Handle(LogOutCommand command, CancellationToken cancellationToken)
    {
        command.Player.Logout(command.Force);
        return Unit.ValueTask;
    }
}