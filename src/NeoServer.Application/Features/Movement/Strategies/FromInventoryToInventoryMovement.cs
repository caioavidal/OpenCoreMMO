using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Location;
using NeoServer.Networking.Packets.Incoming;

namespace NeoServer.Application.Features.Movement.Strategies;

public class FromInventoryToInventoryMovement:IItemMovement
{
    public void Handle(IPlayer player, ItemThrowPacket itemThrow)
    {
        var item = player.Inventory[itemThrow.FromLocation.Slot];
        if (item is null) return;

        if (!item.IsPickupable) return;

        player.MoveItem(item, player.Inventory, player.Inventory, itemThrow.Count,
            (byte)itemThrow.FromLocation.Slot, (byte)itemThrow.ToLocation.Slot);
    }

    public string MovementKey => $"{LocationType.Slot.ToString()}-{LocationType.Slot.ToString()}";
}