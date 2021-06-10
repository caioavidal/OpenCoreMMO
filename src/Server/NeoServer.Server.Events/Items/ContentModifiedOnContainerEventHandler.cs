using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Networking.Packets.Outgoing.Item;
using NeoServer.Networking.Packets.Outgoing.Npc;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Events.Items
{
    public class ContentModifiedOnContainerEventHandler
    {
        private readonly IGameServer game;

        public ContentModifiedOnContainerEventHandler(IGameServer game)
        {
            this.game = game;
        }

        public void Execute(IPlayer player, ContainerOperation operation, byte containerId, byte slotIndex, IItem item)
        {
            if (game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection))
            {
                switch (operation)
                {
                    case ContainerOperation.ItemRemoved:
                        connection.OutgoingPackets.Enqueue(new RemoveItemContainerPacket(containerId, slotIndex, item));
                        break;
                    case ContainerOperation.ItemAdded:
                        connection.OutgoingPackets.Enqueue(new AddItemContainerPacket(containerId, item));
                        break;
                    case ContainerOperation.ItemUpdated:
                        connection.OutgoingPackets.Enqueue(new UpdateItemContainerPacket(containerId, slotIndex, item));
                        break;
                }

                if (player.Containers[containerId]?.Parent == player && player.Shopping)
                    connection.OutgoingPackets.Enqueue(new SaleItemListPacket(player,
                        player.TradingWithNpc?.ShopItems?.Values));

                connection.Send();
            }
        }
    }

    public enum ContainerOperation
    {
        ItemRemoved,
        ItemAdded,
        ItemUpdated
    }
}