using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Game.Items.Items.UsableItems
{
    public class TransformerUsableItem : UseableOnItem
    {
        private readonly IItemFactory _itemFactory;
        public TransformerUsableItem(IItemType type, Location location, IItemFactory itemFactory) : base(type, location)
        {
            _itemFactory = itemFactory;
        }

        public override void UseOn(IPlayer player, IMap map, IThing thing)
        {
            if (thing is not IItem item) return;
            if (item.Metadata.Attributes.GetTransformationItem() == default) return;

            if (Metadata.OnUse?.GetAttribute<ushort>(Common.ItemAttribute.UseOn) != item.Metadata.TypeId) return;

            if (thing is IGround ground && map[ground.Location] is IDynamicTile tile)
            {
                var itemCreated =  _itemFactory.Create(item.Metadata.Attributes.GetTransformationItem(), ground.Location, null);

                tile.RemoveItem(ground,1,0, out var removedItem);
                tile.AddItem(itemCreated);
            }

            base.UseOn(player, map, thing);
        }

        public static new bool IsApplicable(IItemType type) => UseableOnItem.IsApplicable(type) && (type.OnUse?.HasAttribute(Common.ItemAttribute.TransformTo) ?? false);
    }
}
