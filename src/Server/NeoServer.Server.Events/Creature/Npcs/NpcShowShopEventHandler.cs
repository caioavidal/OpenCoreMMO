using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing.Npc;
using NeoServer.Server.Contracts;
using NeoServer.Game.Contracts.Creatures;
using System.Collections.Generic;

namespace NeoServer.Server.Events.Creature.Npcs
{
    public class NpcShowShopEventHandler
    {
        private readonly IGameServer game;

        public NpcShowShopEventHandler(IGameServer game)
        {
            this.game = game;
        }

        public void Execute(INpc npc, ISociableCreature to, IEnumerable<IShopItem> shopItems)
        {
            if (!game.CreatureManager.GetPlayerConnection(to.CreatureId, out var connection)) return;

            connection.OutgoingPackets.Enqueue(new OpenShopPacket(shopItems));

            if(to is IPlayer player && player.Shopping) connection.OutgoingPackets.Enqueue(new SaleItemListPacket(player, shopItems));
            connection.Send();
        }
    }
}
