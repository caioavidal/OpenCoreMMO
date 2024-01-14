using Mediator;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Application.Features.Session.LogOut;

public record LogOutCommand(IPlayer Player, bool Force) : ICommand;
public class LogOutCommandHandler : ICommandHandler<LogOutCommand>
{
    private readonly IGameServer _gameServer;
    public LogOutCommandHandler(IGameServer gameServer) => _gameServer = gameServer;

    public ValueTask<Unit> Handle(LogOutCommand command, CancellationToken cancellationToken)
    {
        if (!command.Player.Logout(command.Force) && !command.Force) return Unit.ValueTask;

        _gameServer.CreatureManager.RemovePlayer(command.Player);

        return Unit.ValueTask;
    }
}