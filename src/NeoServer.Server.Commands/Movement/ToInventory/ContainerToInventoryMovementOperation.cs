using NeoServer.Game.Common.Location;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Commands.Movement
{
    public class ContainerToInventoryMovementOperation
    {
        public static void Execute(IPlayer player, ItemThrowPacket itemThrow)
        {
            var container = player.Containers[itemThrow.FromLocation.ContainerId];

            if (container[itemThrow.FromLocation.ContainerSlot] is not IPickupable item) return;

            if (player.Inventory.CanAddItemToSlot(itemThrow.ToLocation.Slot, item).IsSuccess is false) return;

            var removedItem = container.RemoveItem((byte)itemThrow.FromLocation.ContainerSlot, itemThrow.Count) as IPickupable;

            var result = player.Inventory.TryAddItemToSlot(itemThrow.ToLocation.Slot, removedItem);

            if (result.IsSuccess is false || result.Value is not IPickupable returnedItem) return;

            container.TryAddItem(returnedItem);
        }

        public static bool IsApplicable(ItemThrowPacket itemThrowPacket) =>
          itemThrowPacket.FromLocation.Type == LocationType.Container
          && itemThrowPacket.ToLocation.Type == LocationType.Slot;
    }
}
