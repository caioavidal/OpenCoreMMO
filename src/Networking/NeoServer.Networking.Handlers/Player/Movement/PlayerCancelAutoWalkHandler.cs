using NeoServer.Game.Common.Contracts.World;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Player.Movement;

public class PlayerCancelAutoWalkHandler : PacketHandler
{
    private readonly IGameServer game;
    private readonly IMap map;

    public PlayerCancelAutoWalkHandler(IGameServer game, IMap map)
    {
        this.game = game;
        this.map = map;
    }

    public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        if (game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player))
            game.Dispatcher.AddEvent(new Event(player.CancelWalk));
    }
}