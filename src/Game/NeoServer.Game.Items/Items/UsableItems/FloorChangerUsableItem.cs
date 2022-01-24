using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Items.UsableItems;

public class FloorChangerUsableItem : UsableOnItem
{
    public FloorChangerUsableItem(IItemType type, Location location) : base(type, location)
    {
    }

    public override bool Use(ICreature usedBy, IItem item)
    {
        if (usedBy is not IPlayer player) return false;
        if (Metadata.OnUse?.GetAttribute<ushort>(ItemAttribute.UseOn) != item.Metadata.TypeId) return false;

        if (Metadata.OnUse?.GetAttribute(ItemAttribute.FloorChange) == "up")
        {
            var toLocation = new Location(item.Location.X, item.Location.Y, (byte)(item.Location.Z - 1));

            player.TeleportTo(toLocation);
            return true;
        }

        return false;
    }

    public new static bool IsApplicable(IItemType type)
    {
        return UsableOnItem.IsApplicable(type) &&
               (type.OnUse?.HasAttribute(ItemAttribute.FloorChange) ?? false);
    }
}