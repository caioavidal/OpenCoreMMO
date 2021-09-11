using System.Linq;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.DataStore;
using NeoServer.Game.Items.Items.Attributes;

namespace NeoServer.Game.Items.Factories.AttributeFactory
{
    public class ProtectionFactory : IFactory
    {
        private readonly ItemTypeStore _itemTypeStore;

        public ProtectionFactory(ItemTypeStore itemTypeStore)
        {
            _itemTypeStore = itemTypeStore;
        }

        public event CreateItem OnItemCreated;

        public IProtection Create(IItemType itemType)
        {
            if (itemType.Attributes.DamageProtection is not { } damageProtection) return null;
            if (!damageProtection.Any()) return null;

            return new Protection(damageProtection);
        }
    }
}