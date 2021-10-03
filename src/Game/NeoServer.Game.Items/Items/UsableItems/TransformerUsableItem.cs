using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Items.UsableItems
{
    public class TransformerUsableItem : UsableOnItem
    {
        public TransformerUsableItem(IItemType type, Location location) : base(type, location)
        {
        }

        public override bool Use(ICreature usedBy, IItem item)
        {
            return true;
            //    if (thing is not IItem item) return;
            //    if (item.Metadata.Attributes.GetTransformationItem() == default) return;

            //    if (Metadata.OnUse?.GetAttribute<ushort>(Common.ItemAttribute.UseOn) != item.Metadata.TypeId) return;

            //    if (thing is IGround ground && map[ground.Location] is IDynamicTile tile)
            //    {
            //        var itemCreated =  ItemFactory.Instance.Create(item.Metadata.Attributes.GetTransformationItem(), ground.Location, null);

            //        tile.RemoveItem(ground,1,0, out var removedItem);
            //        tile.AddItem(itemCreated);
            //    }

            //    base.UseOn(player, map, thing);
        }

        public new static bool IsApplicable(IItemType type)
        {
            return UsableOnItem.IsApplicable(type) &&
                   (type.OnUse?.HasAttribute(ItemAttribute.TransformTo) ?? false);
        }
    }
}