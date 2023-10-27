using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Location;
using NeoServer.Networking.Packets.Incoming;

namespace NeoServer.Application.Features.Movement.Strategies;

public class FromContainerToInventoryMovement : IItemMovement
{
    public void Handle(IPlayer player, ItemThrowPacket itemThrow)
    {
        var container = player.Containers[itemThrow.FromLocation.ContainerId];

        var item = container[itemThrow.FromLocation.ContainerSlot];

        if (item is null) return;

        if (!item.IsPickupable) return;

        player.MoveItem(item, container, player.Inventory, itemThrow.Count,
            (byte)itemThrow.FromLocation.ContainerSlot, (byte)itemThrow.ToLocation.Slot);
    }

    public string MovementKey => $"{LocationType.Container.ToString()}-{LocationType.Slot.ToString()}";
}