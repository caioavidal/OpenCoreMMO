using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Bases;

namespace NeoServer.Game.Items.Items.UsableItems;

public abstract class UsableOnItem : MovableItem, IPickupable, IUsableOnItem
{
    public virtual bool AllowUseOnDistance => false;
        
    protected UsableOnItem(IItemType type, Location location) : base(type, location)
    {
    }

    public abstract bool Use(ICreature usedBy, IItem onItem);

    public virtual bool CanUse(ICreature usedBy, IItem onItem)
    {
        if (!AllowUseOnDistance && !usedBy.Location.IsNextTo(onItem.Location)) return false;
        return usedBy.Location.SameFloorAs(onItem.Location);
    }

    public static bool IsApplicable(IItemType type)
    {
        return type.Flags.Contains(ItemFlag.Useable) && type.Flags.Contains(ItemFlag.Pickupable);
    }
}