using NeoServer.Game.Common.Players;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Networking.Packets.Outgoing.Npc;
using NeoServer.Server.Contracts;
using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Server.Events.Player
{
    public class PlayerChangedInventoryEventHandler
    {
        private readonly IGameServer game;

        public PlayerChangedInventoryEventHandler(IGameServer game)
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
