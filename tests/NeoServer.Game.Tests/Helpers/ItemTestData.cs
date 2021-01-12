using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.Items.Types.Containers;
using NeoServer.Game.Items.Items;

namespace NeoServer.Game.Items.Tests
{
    public class ItemTestData
    {
        public static Container CreateContainer(byte capacity = 6)
        {
            var itemType = new ItemType();
            itemType.Attributes.SetAttribute(Common.ItemAttribute.Capacity, capacity);

            return new Container(itemType, new Location(100, 100, 7));
        }
        public static IPickupableContainer CreatePickupableContainer(byte capacity = 6)
        {
            var itemType = new ItemType();
            itemType.Attributes.SetAttribute(Common.ItemAttribute.Capacity, capacity);
            itemType.Attributes.SetAttribute(Common.ItemAttribute.Weight, 20);

            return new PickupableContainer(itemType, new Location(100, 100, 7));
        }
        public static PickupableContainer CreateBackpack()
        {
            var itemType = new ItemType();
            itemType.Attributes.SetAttribute(Common.ItemAttribute.Capacity, 20);
            itemType.Attributes.SetAttribute(Common.ItemAttribute.Weight, 20);

            itemType.Attributes.SetAttribute(Common.ItemAttribute.BodyPosition, "backpack");

            return new PickupableContainer(itemType, new Location(100, 100, 7));
        }

        public static ICumulative CreateCumulativeItem(ushort id, byte amount)
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.SetName("item");
            type.Attributes.SetAttribute(Common.ItemAttribute.Weight, 1);

            return new Cumulative(type, new Location(100, 100, 7), amount);
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

            return new MeleeWeapon(type, new Location(100, 100, 7));
        }
        public static IPickupable CreateWeaponItem(ushort id, string weaponType, bool twoHanded = false)
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.SetName("item");
            type.Attributes.SetAttribute(Common.ItemAttribute.WeaponType, weaponType);
            type.Attributes.SetAttribute(Common.ItemAttribute.Weight, 40);

            if (twoHanded)
                type.Attributes.SetAttribute(Common.ItemAttribute.BodyPosition, "two-handed");
            return new MeleeWeapon(type, new Location(100, 100, 7));
        }
        public static IPickupable CreateThrowableDistanceItem(ushort id, byte amount, bool twoHanded = false)
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.SetName("item");

            type.Attributes.SetAttribute(Common.ItemAttribute.WeaponType, "distance");
            type.Attributes.SetAttribute(Common.ItemAttribute.Weight, 40);

            return new ThrowableDistanceWeapon(type, new Location(100, 100, 7), amount);
        }

        public static object CreateRing(ushort id)
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.Attributes.SetAttribute(Common.ItemAttribute.BodyPosition, "ring");
            type.SetName("item");

            return new Ring(type, new Location(100, 100, 7));
        }
        public static object CreateNecklace(ushort id)
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.Attributes.SetAttribute(Common.ItemAttribute.BodyPosition, "necklace");
            type.SetName("item");

            return new Necklace(type, new Location(100, 100, 7));
        }

        public static IPickupable CreateBodyEquipmentItem(ushort id, string slot, string weaponType = "")
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.Attributes.SetAttribute(Common.ItemAttribute.BodyPosition, slot);
            type.Attributes.SetAttribute(Common.ItemAttribute.WeaponType, weaponType);
            type.Attributes.SetAttribute(Common.ItemAttribute.Weight, 40);
            type.SetName("item");

            return new BodyDefenseEquimentItem(type, new Location(100, 100, 7));
        }

        public static IPickupable CreateAmmoItem(ushort id, byte amount)
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.SetName("item");
            type.Attributes.SetAttribute(Common.ItemAttribute.WeaponType, "ammunition");
            type.Attributes.SetAttribute(Common.ItemAttribute.BodyPosition, "ammo");
            type.Attributes.SetAttribute(Common.ItemAttribute.Weight, 1);

            return new AmmoItem(type, new Location(100, 100, 7), amount);
        }

        public static IItem CreateTopItem(ushort id, byte topOrder)
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.SetName("item");

            if (topOrder == 1)
            {
                type.SetFlag(Common.ItemFlag.AlwaysOnTop);
            }
            else
            {
                type.SetFlag(Common.ItemFlag.Bottom);
            }

            return new Item(type, new Location(100, 100, 7));
        }

    }
}
