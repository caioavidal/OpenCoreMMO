using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Items.Items;

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
            itemType.Attributes.SetAttribute(Enums.ItemAttribute.Weight, 2000);

            return new PickupableContainer(itemType, new Location(100, 100, 7));
        }
        public static PickupableContainer CreateBackpack()
        {
            var itemType = new ItemType();
            itemType.Attributes.SetAttribute(Enums.ItemAttribute.Capacity, 20);
            itemType.Attributes.SetAttribute(Enums.ItemAttribute.Weight, 2000);

            itemType.Attributes.SetAttribute(Enums.ItemAttribute.BodyPosition, "backpack");

            return new PickupableContainer(itemType, new Location(100, 100, 7));
        }

        public static ICumulativeItem CreateCumulativeItem(ushort id, byte amount)
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.SetName("item");
            type.Attributes.SetAttribute(Enums.ItemAttribute.Weight, 100);

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

            return new MeleeWeapon(type, new Location(100, 100, 7));
        }
        public static IPickupable CreateWeaponItem(ushort id, string weaponType, bool twoHanded = false)
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.SetName("item");
            type.Attributes.SetAttribute(Enums.ItemAttribute.WeaponType, weaponType);
            type.Attributes.SetAttribute(Enums.ItemAttribute.Weight, 4000);

            if (twoHanded)
                type.Attributes.SetAttribute(Enums.ItemAttribute.BodyPosition, "two-handed");
            return new MeleeWeapon(type, new Location(100, 100, 7));
        }
        public static IPickupable CreateThrowableDistanceItem(ushort id, byte amount, bool twoHanded = false)
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.SetName("item");

            type.Attributes.SetAttribute(Enums.ItemAttribute.WeaponType, "distance");
            type.Attributes.SetAttribute(Enums.ItemAttribute.Weight, 4000);

            return new ThrowableDistanceWeapon(type, new Location(100, 100, 7), amount);
        }

        public static object CreateRing(ushort id)
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

        public static IPickupable CreateBodyEquipmentItem(ushort id, string slot, string weaponType = "")
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.Attributes.SetAttribute(Enums.ItemAttribute.BodyPosition, slot);
            type.Attributes.SetAttribute(Enums.ItemAttribute.WeaponType, weaponType);
            type.Attributes.SetAttribute(Enums.ItemAttribute.Weight, 4000);
            type.SetName("item");

            return new BodyDefenseEquimentItem(type, new Location(100, 100, 7));
        }

        public static IPickupable CreateAmmoItem(ushort id, byte amount)
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.SetName("item");
            type.Attributes.SetAttribute(Enums.ItemAttribute.WeaponType, "ammunition");
            type.Attributes.SetAttribute(Enums.ItemAttribute.BodyPosition, "ammo");
            type.Attributes.SetAttribute(Enums.ItemAttribute.Weight, 100);

            return new AmmoItem(type, new Location(100, 100, 7), amount);
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
