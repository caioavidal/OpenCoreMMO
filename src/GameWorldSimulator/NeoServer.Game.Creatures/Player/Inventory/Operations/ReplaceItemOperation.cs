using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Game.Creatures.Player.Inventory.Operations;

public static class ReplaceItemOperation
{
    public static bool Replace(Inventory inventory, IItem fromItem, IItemType toItemType)
    {
        if (toItemType is null) return false;
        if (fromItem.Metadata.Group != toItemType.Group) return false;

        fromItem.UpdateMetadata(toItemType);
        if (fromItem is IDressable dressable) dressable.DressedIn(inventory.Owner);
        return true;
    }
}