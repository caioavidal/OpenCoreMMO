using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Networking.Packets.Outgoing.Npc;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Events.Creature.Npcs
{
    public class NpcShowShopEventHandler
    {
        private readonly IGameServer _game;
        private readonly ICoinTypeStore _coinTypeStore;

        public NpcShowShopEventHandler(IGameServer game, ICoinTypeStore coinTypeStore)
        {
            this._game = game;
            _coinTypeStore = coinTypeStore;
        }

        public void Execute(INpc npc, ISociableCreature to, IEnumerable<IShopItem> shopItems)
        {
            if (!_game.CreatureManager.GetPlayerConnection(to.CreatureId, out var connection)) return;

            connection.OutgoingPackets.Enqueue(new OpenShopPacket(shopItems));

            if (to is IPlayer player && player.Shopping)
                connection.OutgoingPackets.Enqueue(new SaleItemListPacket(player, shopItems,_coinTypeStore));
            connection.Send();
        }
    }
}