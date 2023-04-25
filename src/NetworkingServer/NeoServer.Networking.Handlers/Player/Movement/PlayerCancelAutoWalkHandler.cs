using NeoServer.Game.Common.Contracts.World;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Player.Movement;

public class PlayerCancelAutoWalkHandler : PacketHandler
{
    private readonly IGameServer _game;
    private readonly IMap _map;

    public PlayerCancelAutoWalkHandler(IGameServer game, IMap map)
    {
        _game = game;
        _map = map;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        if (_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player))
            _game.Dispatcher.AddEvent(new Event(player.StopWalking));
    }
}