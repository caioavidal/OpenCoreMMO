using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.DataStore;
using NeoServer.Game.Items.Items;
using NeoServer.Game.Items.Items.Protections;

namespace NeoServer.Game.Items.Factories
{
    public class AccessoryFactory : IFactory
    {
        private readonly ItemTypeStore _itemTypeStore;
        public event CreateItem OnItemCreated;

        public AccessoryFactory(ItemTypeStore itemTypeStore)
        {
            _itemTypeStore = itemTypeStore;
        }

        public Accessory Create(IItemType itemType, Location location)
        {
            if (Ring.IsApplicable(itemType))
            {
                Func<IItemType> transformEquipItem = () => _itemTypeStore.Get(itemType.Attributes.GetAttribute<ushort>(ItemAttribute.TransformEquipTo));
                return new Ring(itemType, location)
                {
                    TransformEquipItem = transformEquipItem,
                    TransformDequipItem = () => transformEquipItem?.Invoke() is { } type ?  _itemTypeStore.Get(type.Attributes.GetAttribute<ushort>(ItemAttribute.TransformDequipTo)) : null,
                    DecaysTo = ()=> _itemTypeStore.Get(itemType.Attributes.GetAttribute<ushort>(ItemAttribute.DecayTo))
                };
            }
            if (Necklace.IsApplicable(itemType))
            {
                Func<IItemType> transformEquipItem = () => _itemTypeStore.Get(itemType.Attributes.GetAttribute<ushort>(ItemAttribute.TransformEquipTo));

                return new Necklace(itemType, location)
                {
                    TransformEquipItem = transformEquipItem,
                    TransformDequipItem = () => transformEquipItem?.Invoke() is { } type ? _itemTypeStore.Get(type.Attributes.GetAttribute<ushort>(ItemAttribute.TransformDequipTo)) : null,
                    DecaysTo = () => _itemTypeStore.Get(itemType.Attributes.GetAttribute<ushort>(ItemAttribute.DecayTo))
                };
            }

            return null;
        }
    }
}
