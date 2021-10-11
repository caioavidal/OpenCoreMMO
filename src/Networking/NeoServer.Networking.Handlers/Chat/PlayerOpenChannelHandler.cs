using System.Linq;
using Microsoft.Diagnostics.Runtime.ICorDebug;
using NeoServer.Game.Common.Contracts.Chats;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Networking.Packets.Incoming.Chat;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Chat
{
    public class PlayerOpenChannelHandler : PacketHandler
    {
        private readonly IGameServer _game;
        private readonly IChatChannelStore _chatChannelStore;
        private readonly IGuildStore _guildStore;

        public PlayerOpenChannelHandler(IGameServer game, IChatChannelStore chatChannelStore, IGuildStore guildStore)
        {
            _game = game;
            _chatChannelStore = chatChannelStore;
            _guildStore = guildStore;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var channelPacket = new OpenChannelPacket(message);
            if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

            IChatChannel channel = null;
            if (_chatChannelStore.Get(channelPacket.ChannelId) is { } publicChannel)
                channel = publicChannel;
            if (player.PersonalChannels?.FirstOrDefault(x => x.Id == channelPacket.ChannelId) is { } personalChannel) channel = personalChannel;
            if (player.GetPrivateChannels(_guildStore)?.FirstOrDefault(x => x.Id == channelPacket.ChannelId) is { } privateChannel) channel = privateChannel;

            if (channel is null) return;

            _game.Dispatcher.AddEvent(new Event(() => player.JoinChannel(channel)));
        }
    }
}