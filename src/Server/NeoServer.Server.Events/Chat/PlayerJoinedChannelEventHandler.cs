using NeoServer.Game.Contracts.Chats;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Events
{
    public class PlayerJoinedChannelEventHandler
    {
        private readonly IGameServer game;

        public PlayerJoinedChannelEventHandler(IGameServer game)
        {
            this.game = game;
        }
        public void Execute(IPlayer player, IChatChannel channel)
        {
            if (channel is null) return;
            if (player is null) return;
            if (!game.CreatureManager.GetPlayerConnection(player.CreatureId, out IConnection connection)) return;

            connection.OutgoingPackets.Enqueue(new PlayerOpenChannelPacket(channel.Id, channel.Name));

            if (!string.IsNullOrWhiteSpace(channel.Description))
            {
                connection.OutgoingPackets.Enqueue(new MessageToChannelPacket(null, NeoServer.Game.Common.Talks.SpeechType.ChannelWhiteText, channel.Description, channel.Id));
            }

            connection.Send();
        }
    }
}
