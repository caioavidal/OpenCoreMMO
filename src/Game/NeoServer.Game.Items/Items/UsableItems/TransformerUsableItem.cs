using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Items.UsableItems;

public class TransformerUsableItem : UsableOnItem
{
    public TransformerUsableItem(IItemType type, Location location) : base(type, location)
    {
    }

    public override bool AllowUseOnDistance => false;

    public override bool Use(ICreature usedBy, IItem item)
    {
        if (usedBy is not IPlayer player) return false;
        item.Transform(player);
        return true;
    }
    
    public new static bool IsApplicable(IItemType type)
    {
        return UsableOnItem.IsApplicable(type) &&
               (type.OnUse?.HasAttribute(ItemAttribute.TransformTo) ?? false);
    }
}