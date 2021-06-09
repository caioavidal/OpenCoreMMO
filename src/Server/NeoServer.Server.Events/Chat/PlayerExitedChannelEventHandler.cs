using NeoServer.Game.Contracts.Chats;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts;

namespace NeoServer.Server.Events
{
    public class PlayerExitedChannelEventHandler
    {
        private readonly IGameServer game;

        public PlayerExitedChannelEventHandler(IGameServer game)
        {
            this.game = game;
        }

        public void Execute(IPlayer player, IChatChannel channel)
        {
            if (channel is null) return;
            if (player is null) return;
            if (!game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection)) return;

            connection.OutgoingPackets.Enqueue(new PlayerCloseChannelPacket(channel.Id));
            connection.Send();
        }
    }
}