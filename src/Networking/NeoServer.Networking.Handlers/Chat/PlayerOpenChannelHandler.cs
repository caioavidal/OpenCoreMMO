using NeoServer.Game.Contracts.Chats;
using NeoServer.Game.DataStore;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Tasks;
using System.Linq;

namespace NeoServer.Server.Handlers.Player
{
    public class PlayerOpenChannelHandler : PacketHandler
    {
        private readonly Game game;
        public PlayerOpenChannelHandler(Game game)
        {
            this.game = game;
        }
        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var channelPacket = new OpenChannelPacket(message);
            if (!game.CreatureManager.TryGetPlayer(connection.PlayerId, out var player)) return;

            IChatChannel channel = null;
            if (ChatChannelStore.Data.Get(channelPacket.ChannelId) is IChatChannel publicChannel) channel = publicChannel;
            if (player.PersonalChannels?.FirstOrDefault(x => x.Id == channelPacket.ChannelId) is IChatChannel personalChannel) channel = personalChannel;

            if (channel is null) return;

            game.Dispatcher.AddEvent(new Event(() => player.JoinChannel(channel)));
        }
    }
}

