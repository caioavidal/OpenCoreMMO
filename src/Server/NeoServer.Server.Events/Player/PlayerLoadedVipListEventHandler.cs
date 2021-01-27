using NeoServer.Data.Interfaces;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Server.Events.Player
{
    public class PlayerLoadedVipListEventHandler
    {
        private readonly Game game;
        
        public PlayerLoadedVipListEventHandler(Game game)
        {
            this.game = game;
        }

        public void Execute(IPlayer player, IEnumerable<(uint, string)> vipList)
        {
            if (!game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection)) return;

            foreach (var vip in vipList)
            {
                var isOnline = game.CreatureManager.TryGetLoggedPlayer(vip.Item1, out var loggedPlayer);
                connection.OutgoingPackets.Enqueue(new PlayerAddVipPacket(vip.Item1, vip.Item2, isOnline));
            }
            connection.Send();
        }
    }
}
