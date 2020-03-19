using NeoServer.Server.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Model.Items
{
    public class ItemFactory
    {
        public static Item Create(ushort typeId)
        {
            if (typeId < 100 || !ItemData.Items.ContainsKey(typeId))
            {
                return null;
                // throw new ArgumentException("Invalid type.", nameof(typeId));
            }

            if (ItemData.Items[typeId].Flags.Contains(ItemFlag.Container) || ItemData.Items[typeId].Flags.Contains(ItemFlag.Chest))
            {
                return new Container(ItemData.Items[typeId]);
            }

            return new Item(ItemData.Items[typeId]);
        }
    }
}
