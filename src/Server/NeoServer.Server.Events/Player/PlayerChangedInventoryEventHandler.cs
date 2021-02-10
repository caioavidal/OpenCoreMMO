using NeoServer.Game.Common.Players;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Networking.Packets.Outgoing.Npc;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Events.Player
{
    public class PlayerChangedInventoryEventHandler
    {
        private readonly Game game;

        public PlayerChangedInventoryEventHandler(Game game)
        {
            this.game = game;
        }
        public void Execute(IPlayer player, Slot slot)
        {
            if (game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection))
            {
                connection.OutgoingPackets.Enqueue(new PlayerInventoryItemPacket(player.Inventory, slot));

                if (player.Shopping) connection.OutgoingPackets.Enqueue(new SaleItemListPacket(player, player.TradingWithNpc?.ShopItems?.Values));

                connection.Send();
            }
        }
    }
}
