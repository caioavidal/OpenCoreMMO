using NeoServer.Game.Common.Location;
using NeoServer.Game.Contracts;
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


            var result = player.Inventory.TryAddItemToSlot(itemThrow.ToLocation.Slot, item);
            if (result.Success is false) return;

            container.RemoveItem((byte)itemThrow.FromLocation.ContainerSlot, itemThrow.Count);
            container.TryAddItem(result.Value);

        }

        public static bool IsApplicable(ItemThrowPacket itemThrowPacket) =>
          itemThrowPacket.FromLocation.Type == LocationType.Container
          && itemThrowPacket.ToLocation.Type == LocationType.Slot;
    }
}
