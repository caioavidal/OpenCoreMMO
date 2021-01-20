using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Common.Combat;
using NeoServer.Game.Contracts.Chats;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Events
{
    public class PlayerJoinedChannelEventHandler
    {
        private readonly Game game;

        public PlayerJoinedChannelEventHandler(Game game)
        {
            this.game = game;
        }
        public void Execute(IPlayer player, IChatChannel channel)
        {
            if (channel is null) return;
            if (player is null) return;
            if (!game.CreatureManager.GetPlayerConnection(player.CreatureId, out IConnection connection)) return;
            
            connection.OutgoingPackets.Enqueue(new PlayerOpenChannelPacket(channel.Id, channel.Name));
            connection.Send();
        }
    }
}
