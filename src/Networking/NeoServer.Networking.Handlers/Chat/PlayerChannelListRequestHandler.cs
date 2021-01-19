using NeoServer.Game.DataStore;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Tasks;
using System.Linq;

namespace NeoServer.Server.Handlers.Player
{
    public class PlayerChannelListRequestHandler : PacketHandler
    {
        private readonly Game game;
        public PlayerChannelListRequestHandler(Game game)
        {
            this.game = game;
        }
        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            if (!game.CreatureManager.TryGetPlayer(connection.PlayerId, out var player)) return;

            connection.OutgoingPackets.Enqueue(new PlayerChannelListPacket(ChatChannelStore.Data.All.ToArray()));
            connection.Send();
        }
    }
}
