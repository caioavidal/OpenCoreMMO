using NeoServer.Game.Contracts.Item;
using NeoServer.Game.Enums;
using NeoServer.Server.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Model.Items
{
    public class ItemFactory
    {
        public static IItem Create(ushort typeId)
        {
            if (typeId < 100 || !ItemData.Items.ContainsKey(typeId))
            {
                return null;
                // throw new ArgumentException("Invalid type.", nameof(typeId));
            }

            var item = ItemData.Items[typeId];

            if (item.Group == ItemGroup.ITEM_GROUP_DEPRECATED)
            {
                return null;
            }

            if (item.Flags.Contains(ItemFlag.Container) || item.Flags.Contains(ItemFlag.Chest))
            {
                return new Container(ItemData.Items[typeId]);
            }

            return new Item(ItemData.Items[typeId]);
        }
    }
}
