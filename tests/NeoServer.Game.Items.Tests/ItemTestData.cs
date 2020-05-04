using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Items.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Items.Tests
{
    public class ItemTestData
    {
        public static Container CreateContainer(byte capacity = 6)
        {
            var itemType = new ItemType();
            itemType.Attributes.SetAttribute(Enums.ItemAttribute.Capacity, capacity);

            return new Container(itemType);
        }

        public static ICumulativeItem CreateCumulativeItem(ushort id, byte amount)
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.SetName("item");

            return new CumulativeItem(type, new Location(100, 100, 7), amount);
        }

        public static Item CreateRegularItem(ushort id)
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.SetName("item");

            return new Item(type, new Location(100, 100, 7));
        }
        public static IItem CreateMoveableItem(ushort id)
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.SetName("item");

            return new WeaponItem(type);
        }

        public static IItem CreateTopItem(ushort id, byte topOrder)
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.SetName("item");

            if (topOrder == 1)
            {
                type.SetFlag(Enums.ItemFlag.AlwaysOnTop);
            }
            else
            {
                type.SetFlag(Enums.ItemFlag.Bottom);
            }

            return new Item(type, new Location(100, 100, 7));
        }
    }
}
