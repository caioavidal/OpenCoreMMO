using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Location;
using NeoServer.Networking.Packets.Incoming;

namespace NeoServer.Application.Features.Movement.Strategies;

public class FromInventoryToContainerMovement:IItemMovement
{
    public void Handle(IPlayer player, ItemThrowPacket itemThrow)
    {
        var container = player.Containers[itemThrow.ToLocation.ContainerId];

        if (container is null) return;

        var item = player.Inventory[itemThrow.FromLocation.Slot];

        if (item is null) return;
        if (!item.IsPickupable) return;

        player.MoveItem(item, player.Inventory, container, itemThrow.Count, (byte)itemThrow.FromLocation.Slot,
            (byte)itemThrow.ToLocation.ContainerSlot);
    }

    public string MovementKey => $"{LocationType.Slot.ToString()}-{LocationType.Container.ToString()}";
}