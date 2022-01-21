using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Networking.Packets.Outgoing.Npc;
using NeoServer.Networking.Packets.Outgoing.Player;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Events.Player
{
    public class PlayerChangedInventoryEventHandler
    {
        private readonly IGameServer game;
        private readonly ICoinTypeStore _coinTypeStore;

        public PlayerChangedInventoryEventHandler(IGameServer game, ICoinTypeStore coinTypeStore)
        {
            this.game = game;
            _coinTypeStore = coinTypeStore;
        }

        public void Execute(IPlayer player, Slot slot)
        {
            if (game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection))
            {
                connection.OutgoingPackets.Enqueue(new PlayerInventoryItemPacket(player.Inventory, slot));

                if (player.Shopping)
                    connection.OutgoingPackets.Enqueue(new SaleItemListPacket(player,
                        player.TradingWithNpc?.ShopItems?.Values,_coinTypeStore));

                connection.Send();
            }
        }
    }
}