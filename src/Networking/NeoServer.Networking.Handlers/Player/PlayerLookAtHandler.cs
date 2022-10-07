using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Location;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Player;

public class PlayerLookAtHandler : PacketHandler
{
    private readonly IGameServer game;

    public PlayerLookAtHandler(IGameServer game)
    {
        this.game = game;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var lookAtPacket = new LookAtPacket(message);

        if (game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player))
        {
            if (lookAtPacket.Location.Type == LocationType.Ground)
            {
                if (game.Map[lookAtPacket.Location] is not ITile tile) return;

                game.Dispatcher.AddEvent(new Event(() => player.LookAt(tile)));
            }

            if (lookAtPacket.Location.Type == LocationType.Container)
                game.Dispatcher.AddEvent(new Event(() =>
                    player.LookAt(lookAtPacket.Location.ContainerId, lookAtPacket.Location.ContainerSlot)));
            if (lookAtPacket.Location.Type == LocationType.Slot)
                game.Dispatcher.AddEvent(new Event(() => player.LookAt(lookAtPacket.Location.Slot)));
        }
    }
}