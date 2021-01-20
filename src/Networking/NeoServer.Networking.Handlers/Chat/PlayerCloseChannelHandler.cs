using NeoServer.Game.Contracts.Chats;
using NeoServer.Game.DataStore;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Handlers.Player
{
    public class PlayerCloseChannelHandler : PacketHandler
    {
        private readonly Game game;
        public PlayerCloseChannelHandler(Game game)
        {
            this.game = game;
        }
        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var channelPacket = new OpenChannelPacket(message);
            if (!game.CreatureManager.TryGetPlayer(connection.PlayerId, out var player)) return;

            if(ChatChannelStore.Data.Get(channelPacket.ChannelId) is not IChatChannel channel) return;

            game.Dispatcher.AddEvent(new Event(() => player.ExitChannel(channel)));            
        }
    }
}
