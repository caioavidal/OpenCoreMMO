using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Player;

public class StopAllActionsHandler : PacketHandler
{
    private readonly IGameServer game;

    public StopAllActionsHandler(IGameServer game)
    {
        this.game = game;
    }

    public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        if (game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player))
            game.Dispatcher.AddEvent(new Event(() => player.StopAllActions()));
    }
}