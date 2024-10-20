using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.BuildingBlocks.Infrastructure.Threading.Event;
using NeoServer.Networking.Packets.Network;

namespace NeoServer.PacketHandler.Modules.Players.Walk;

public class PlayerCancelAutoWalkPacketHandler : PacketHandler
{
    private readonly IGameServer _game;

    public PlayerCancelAutoWalkPacketHandler(IGameServer game)
    {
        _game = game;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        if (_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player))
            _game.Dispatcher.AddEvent(new Event(player.StopWalking));
    }
}