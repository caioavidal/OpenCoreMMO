using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Enums.Item;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Enums.Players;
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

            return new Container(itemType, new Location(100, 100, 7));
        }
        public static PickupableContainer CreatePickupableContainer(byte capacity = 6)
        {
            var itemType = new ItemType();
            itemType.Attributes.SetAttribute(Enums.ItemAttribute.Capacity, capacity);
            itemType.Attributes.SetAttribute(Enums.ItemAttribute.Weight, 20);

            return new PickupableContainer(itemType, new Location(100, 100, 7));
        }
        public static Container CreateBackpack()
        {
            var itemType = new ItemType();
            itemType.Attributes.SetAttribute(Enums.ItemAttribute.Capacity, 20);
            itemType.Attributes.SetAttribute(Enums.ItemAttribute.BodyPosition, "backpack");

            return new Container(itemType, new Location(100, 100, 7));
        }

        public static ICumulativeItem CreateCumulativeItem(ushort id, byte amount)
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.SetName("item");
            type.Attributes.SetAttribute(Enums.ItemAttribute.Weight, 1);

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

            return new WeaponItem(type, new Location(100, 100, 7));
        }
        public static IItem CreateWeaponItem(ushort id, string weaponType, bool twoHanded = false)
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.SetName("item");
            type.Attributes.SetAttribute(Enums.ItemAttribute.WeaponType, weaponType);


            if(twoHanded)
                type.Attributes.SetAttribute(Enums.ItemAttribute.BodyPosition, "two-handed");
            return new WeaponItem(type, new Location(100, 100, 7));
        }

        public static object CreateRing(ushort id )
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.Attributes.SetAttribute(Enums.ItemAttribute.BodyPosition, "ring");
            type.SetName("item");

            return new Ring(type, new Location(100, 100, 7));
        }
        public static object CreateNecklace(ushort id)
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.Attributes.SetAttribute(Enums.ItemAttribute.BodyPosition, "necklace");
            type.SetName("item");

            return new Necklace(type, new Location(100, 100, 7));
        }

        public static IItem CreateBodyEquipmentItem(ushort id, string slot,string weaponType = "")
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.Attributes.SetAttribute(Enums.ItemAttribute.BodyPosition, slot);
            type.Attributes.SetAttribute(Enums.ItemAttribute.WeaponType, weaponType);
            type.Attributes.SetAttribute(Enums.ItemAttribute.Weight, 40);
            type.SetName("item");

            return new BodyDefenseEquimentItem(type, new Location(100, 100, 7));
        }

     

        public static IItem CreatAmmoItem(ushort id, byte amount)
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.SetName("item");
            type.Attributes.SetAttribute(Enums.ItemAttribute.WeaponType, "ammunition");
            type.Attributes.SetAttribute(Enums.ItemAttribute.BodyPosition, "ammo");

            return new AmmoItem(type, new Location(100, 100, 7),amount);
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
