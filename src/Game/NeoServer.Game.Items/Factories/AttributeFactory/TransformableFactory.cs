using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Item;
using NeoServer.Game.DataStore;
using NeoServer.Game.Items.Items.Attributes;

namespace NeoServer.Game.Items.Factories.AttributeFactory
{
    public class TransformableFactory : IFactory
    {
        private readonly ItemTypeStore _itemTypeStore;

        public TransformableFactory(ItemTypeStore itemTypeStore)
        {
            _itemTypeStore = itemTypeStore;
        }

        public event CreateItem OnItemCreated;

        public ITransformable Create(IItemType itemType)
        {
            itemType.Attributes.TryGetAttribute<ushort>(ItemAttribute.TransformEquipTo, out var transformEquipTo);

            var transformOnEquipItemType = _itemTypeStore.Get(transformEquipTo);

            ushort transformDequipTo = 0;
            transformOnEquipItemType?.Attributes?.TryGetAttribute(ItemAttribute.TransformDequipTo,
                out transformDequipTo);

            if (transformDequipTo == default && transformEquipTo == default) return null;

            return new Transformable(itemType)
            {
                TransformEquipItem = () => _itemTypeStore.Get(transformEquipTo),
                TransformDequipItem = () => _itemTypeStore.Get(transformDequipTo)
            };
        }
    }
}