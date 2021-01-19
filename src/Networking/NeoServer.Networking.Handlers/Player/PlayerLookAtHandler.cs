using NeoServer.Game.Contracts.World;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Handlers.Player
{
    public class PlayerLookAtHandler : PacketHandler
    {
        private readonly Game game;
        public PlayerLookAtHandler(Game game)
        {
            this.game = game;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var lookAtPacket = new LookAtPacket(message);

            if (game.CreatureManager.TryGetPlayer(connection.PlayerId, out var player))
            {
                if (lookAtPacket.Location.Type == NeoServer.Game.Common.Location.LocationType.Ground)
                {
                    if (game.Map[lookAtPacket.Location] is not ITile tile) return;
                    
                    game.Dispatcher.AddEvent(new Event(() => player.LookAt(tile)));
                }
                if (lookAtPacket.Location.Type == NeoServer.Game.Common.Location.LocationType.Container)
                {
                    game.Dispatcher.AddEvent(new Event(() => player.LookAt(lookAtPacket.Location.ContainerId, lookAtPacket.Location.ContainerSlot)));
                }
                if (lookAtPacket.Location.Type == NeoServer.Game.Common.Location.LocationType.Slot)
                {
                    game.Dispatcher.AddEvent(new Event(() => player.LookAt(lookAtPacket.Location.Slot)));
                }
            }
        }
    }
}
