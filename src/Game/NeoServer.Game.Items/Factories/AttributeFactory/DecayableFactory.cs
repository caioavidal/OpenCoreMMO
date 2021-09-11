using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Item;
using NeoServer.Game.DataStore;
using NeoServer.Game.Items.Items.Attributes;

namespace NeoServer.Game.Items.Factories.AttributeFactory
{
    public class DecayableFactory : IFactory
    {
        private readonly ItemTypeStore _itemTypeStore;

        public DecayableFactory(ItemTypeStore itemTypeStore)
        {
            _itemTypeStore = itemTypeStore;
        }

        public event CreateItem OnItemCreated;

        public IDecayable Create(IItemType itemType)
        {
            if (!itemType.Attributes.TryGetAttribute<ushort>(ItemAttribute.Duration, out var duration)) return null;

            var decaysTo = itemType.Attributes.GetAttribute<ushort>(ItemAttribute.ExpireTarget);
            return new Decayable(() => _itemTypeStore.Get(decaysTo), duration);
        }
    }
}