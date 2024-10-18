using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.BuildingBlocks.Infrastructure.Threading.Event;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Location;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Network;

namespace NeoServer.PacketHandler.Features.Player.LookAt;

public class PlayerLookAtPacketHandler : PacketHandler
{
    private readonly IGameServer _game;

    public PlayerLookAtPacketHandler(IGameServer game)
    {
        _game = game;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var lookAtPacket = new LookAtPacket(message);

        if (_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player))
        {
            if (lookAtPacket.Location.Type == LocationType.Ground)
            {
                if (_game.Map[lookAtPacket.Location] is not ITile tile) return;

                _game.Dispatcher.AddEvent(new Event(() => player.LookAt(tile)));
            }

            if (lookAtPacket.Location.Type == LocationType.Container)
                _game.Dispatcher.AddEvent(new Event(() =>
                    player.LookAt(lookAtPacket.Location.ContainerId, lookAtPacket.Location.ContainerSlot)));

            if (lookAtPacket.Location.Type == LocationType.Slot)
                _game.Dispatcher.AddEvent(new Event(() => player.LookAt(lookAtPacket.Location.Slot)));
        }
    }
}