using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;

namespace NeoServer.Game.Items.Items.UsableItems
{
    public class TransformerUsableItem : UseableOnItem
    {
        public TransformerUsableItem(IItemType type, Location location) : base(type, location) { }
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

        public static new bool IsApplicable(IItemType type) => UseableOnItem.IsApplicable(type) && (type.OnUse?.HasAttribute(Common.ItemAttribute.TransformTo) ?? false);
    }
}
