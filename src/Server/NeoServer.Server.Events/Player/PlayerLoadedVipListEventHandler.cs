using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts;
using System.Collections.Generic;

namespace NeoServer.Server.Events.Player
{
    public class PlayerLoadedVipListEventHandler
    {
        private readonly IGameServer game;
        
        public PlayerLoadedVipListEventHandler(IGameServer game)
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
