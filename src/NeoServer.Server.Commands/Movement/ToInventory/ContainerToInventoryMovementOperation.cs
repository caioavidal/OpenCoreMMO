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

            player.MoveItem(container, player.Inventory, item, itemThrow.Count, (byte)itemThrow.FromLocation.ContainerSlot, (byte)itemThrow.ToLocation.Slot);
        }

        public static bool IsApplicable(ItemThrowPacket itemThrowPacket) =>
          itemThrowPacket.FromLocation.Type == LocationType.Container
          && itemThrowPacket.ToLocation.Type == LocationType.Slot;
    }
}
