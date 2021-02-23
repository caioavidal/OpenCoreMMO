using NeoServer.Game.Contracts.Chats;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;

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
            if (!game.CreatureManager.GetPlayerConnection(player.CreatureId, out IConnection connection)) return;

            connection.OutgoingPackets.Enqueue(new PlayerCloseChannelPacket(channel.Id));
            connection.Send();
        }
    }
}
