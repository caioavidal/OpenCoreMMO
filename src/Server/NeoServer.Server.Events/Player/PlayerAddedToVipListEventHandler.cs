using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Server.Events.Player
{
    public class PlayerAddedToVipListEventHandler
    {
        private readonly Game game;

        public PlayerAddedToVipListEventHandler(Game game)
        {
            this.game = game;
        }

        public void Execute(IPlayer player, uint vipPlayerId, string vipPlayerName)
        {
            if (!game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection)) return;

            var isOnline = game.CreatureManager.TryGetLoggedPlayer(vipPlayerId, out var loggedPlayer);

            connection.OutgoingPackets.Enqueue(new PlayerAddVipPacket(vipPlayerId, vipPlayerName, isOnline));
            connection.Send();

        }
    }
}
