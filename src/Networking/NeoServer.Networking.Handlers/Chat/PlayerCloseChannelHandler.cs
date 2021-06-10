using System.Linq;
using NeoServer.Game.Common.Contracts.Chats;
using NeoServer.Game.DataStore;
using NeoServer.Networking.Packets.Incoming.Chat;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Chat
{
    public class PlayerCloseChannelHandler : PacketHandler
    {
        private readonly IGameServer game;

        public PlayerCloseChannelHandler(IGameServer game)
        {
            this.game = game;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var channelPacket = new OpenChannelPacket(message);
            if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

            IChatChannel channel = null;

            if (ChatChannelStore.Data.Get(channelPacket.ChannelId) is IChatChannel publicChannel)
                channel = publicChannel;
            if (player.PersonalChannels?.FirstOrDefault(x => x.Id == channelPacket.ChannelId) is IChatChannel
                personalChannel) channel = personalChannel;
            if (player.PrivateChannels?.FirstOrDefault(x => x.Id == channelPacket.ChannelId) is IChatChannel
                privateChannel) channel = privateChannel;

            if (channel is null) return;

            game.Dispatcher.AddEvent(new Event(() => player.ExitChannel(channel)));
        }
    }
}