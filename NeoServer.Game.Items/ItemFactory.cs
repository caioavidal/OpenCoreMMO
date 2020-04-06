using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Enums;
using NeoServer.Server.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Items
{
   
    public class ItemFactory
    {
        /// <summary>
        /// Creates a item instance from typeId
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public static IItem Create(ushort typeId)
        {
            if (typeId < 100 || !ItemTypeData.InMemory.ContainsKey(typeId))
            {
                return null;
            }

            var item = ItemTypeData.InMemory[typeId];

            if (item.Group == ItemGroup.ITEM_GROUP_DEPRECATED)
            {
                return null;
            }

            if (item.Flags.Contains(ItemFlag.Container) || item.Flags.Contains(ItemFlag.Chest))
            {
                return new Container(ItemTypeData.InMemory[typeId]);
            }

            return new Item(ItemTypeData.InMemory[typeId]);
        }
    }
}
