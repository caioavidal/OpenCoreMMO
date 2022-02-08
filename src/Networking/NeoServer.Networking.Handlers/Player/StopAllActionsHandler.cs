using NeoServer.Server.Commands.Player;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Player;

public class StopAllActionsHandler : PacketHandler
{
    private readonly IGameServer game;
    private readonly StopAllActionCommand stopAllActionCommand;

    public StopAllActionsHandler(IGameServer game, StopAllActionCommand stopAllActionCommand)
    {
        this.game = game;
        this.stopAllActionCommand = stopAllActionCommand;
    }

    public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        if (game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player))
            game.Dispatcher.AddEvent(new Event(() => stopAllActionCommand.Execute(player)));
    }
}