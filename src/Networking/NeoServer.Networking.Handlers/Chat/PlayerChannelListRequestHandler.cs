using NeoServer.Game.DataStore;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Network;
using System.Linq;

namespace NeoServer.Server.Handlers.Player
{
    public class PlayerChannelListRequestHandler : PacketHandler
    {
        private readonly IGameServer game;
        public PlayerChannelListRequestHandler(IGameServer game)
        {
            this.game = game;
        }
        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

            var channels = ChatChannelStore.Data.All.Where(x => x.PlayerCanJoin(player));
            channels = player.PersonalChannels is null ? channels : channels.Concat(player.PersonalChannels);
            channels = player.PrivateChannels is null ? channels : channels.Concat(player.PrivateChannels);

            connection.OutgoingPackets.Enqueue(new PlayerChannelListPacket(channels.ToArray()));
            connection.Send();
        }
    }
}
