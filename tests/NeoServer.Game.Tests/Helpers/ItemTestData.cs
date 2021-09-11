using System;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Contracts.Items.Types.Runes;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Parsers;
using NeoServer.Game.Items;
using NeoServer.Game.Items.Bases;
using NeoServer.Game.Items.Items;
using NeoServer.Game.Items.Items.Attributes;
using NeoServer.Game.Items.Items.Containers;
using NeoServer.Game.Items.Items.UsableItems.Runes;
using NeoServer.Game.Items.Items.Weapons;

namespace NeoServer.Game.Tests.Helpers
{
    public class ItemTestData
    {
        public static Container CreateContainer(byte capacity = 6)
        {
            var itemType = new ItemType();
            itemType.Attributes.SetAttribute(ItemAttribute.Capacity, capacity);

            return new Container(itemType, new Location(100, 100, 7));
        }

        public static IPickupableContainer CreatePickupableContainer(byte capacity = 6)
        {
            var itemType = new ItemType();
            itemType.Attributes.SetAttribute(ItemAttribute.Capacity, capacity);
            itemType.Attributes.SetAttribute(ItemAttribute.Weight, 20);

            return new PickupableContainer(itemType, new Location(100, 100, 7));
        }

        public static PickupableContainer CreateBackpack()
        {
            var itemType = new ItemType();
            itemType.Attributes.SetAttribute(ItemAttribute.Capacity, 20);
            itemType.Attributes.SetAttribute(ItemAttribute.Weight, 20);

            itemType.Attributes.SetAttribute(ItemAttribute.BodyPosition, "backpack");

            return new PickupableContainer(itemType, new Location(100, 100, 7));
        }

        public static ICumulative CreateCumulativeItem(ushort id, byte amount, string slot = null)
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.SetId(id);
            type.SetName("item");
            type.Attributes.SetAttribute(ItemAttribute.BodyPosition, slot);
            type.Flags.Add(ItemFlag.Stackable);
            type.Attributes.SetAttribute(ItemAttribute.Weight, 1);

            return new Cumulative(type, new Location(100, 100, 7), amount);
        }

        public static Item CreateRegularItem(ushort id)
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.SetId(id);
            type.SetName("item");

            return new Item(type, new Location(100, 100, 7));
        }

        public static IItem CreateMoveableItem(ushort id)
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.SetId(id);

            type.SetName("item");

            return new MeleeWeapon(type, new Location(100, 100, 7));
        }

        public static IPickupable CreateWeaponItem(ushort id, string weaponType, bool twoHanded = false)
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.SetId(id);
            type.SetName("item");
            type.Attributes.SetAttribute(ItemAttribute.WeaponType, weaponType);
            type.Attributes.SetAttribute(ItemAttribute.Weight, 40);

            if (twoHanded)
                type.Attributes.SetAttribute(ItemAttribute.BodyPosition, "two-handed");
            else
                type.Attributes.SetAttribute(ItemAttribute.BodyPosition, "weapon");

            return new MeleeWeapon(type, new Location(100, 100, 7));
        }

        public static IPickupable CreateThrowableDistanceItem(ushort id, byte amount, bool twoHanded = false)
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.SetId(id);
            type.SetName("item");

            type.Attributes.SetAttribute(ItemAttribute.WeaponType, "distance");
            type.Attributes.SetAttribute(ItemAttribute.Weight, 40);

            return new ThrowableDistanceWeapon(type, new Location(100, 100, 7), amount);
        }

        public static IDefenseEquipment CreateDefenseEquipmentItem(ushort id, string slot = "", ushort charges = 10,
            (ItemAttribute, IConvertible)[] attributes = null,
            Func<IItemType> transformOnEquipItem = null, Func<IItemType> transformOnDequipItem = null,
            Func<IItemType> decaysTo = null)
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.SetId(id);
            type.Attributes.SetAttribute(ItemAttribute.BodyPosition, slot);
            type.Attributes.SetAttribute(ItemAttribute.Charges, charges);
            type.SetName("item");

            attributes ??= new (ItemAttribute, IConvertible)[0];
            foreach (var (attributeType, value) in attributes) type.Attributes.SetAttribute(attributeType, value);

            return new BodyDefenseEquipment(type, new Location(100, 100, 7))
            {
                Decayable = type.Attributes.GetAttribute<ushort>(ItemAttribute.Duration) == 0 ? null : new Decayable(decaysTo, type.Attributes.GetAttribute<ushort>(ItemAttribute.Duration)),
                Transformable = new Transformable(type){ TransformEquipItem = transformOnEquipItem, TransformDequipItem = transformOnDequipItem },
                Protection = type.Attributes.DamageProtection is null ? null : new Protection(type.Attributes.DamageProtection),
                SkillBonus = type.Attributes.SkillBonuses is null ? null : new SkillBonus(type.Attributes.SkillBonuses)
            };
        }
        
        public static IPickupable CreateBodyEquipmentItem(ushort id, string slot, string weaponType = "")
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.SetId(id);
            type.Attributes.SetAttribute(ItemAttribute.BodyPosition, slot);
            type.Attributes.SetAttribute(ItemAttribute.WeaponType, weaponType);
            type.Attributes.SetAttribute(ItemAttribute.Weight, 40);
            type.SetName("item");

            return new BodyDefenseEquipment(type, new Location(100, 100, 7));
        }

        public static IPickupable CreateAmmoItem(ushort id, byte amount)
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.SetId(id);
            type.SetName("item");
            type.Attributes.SetAttribute(ItemAttribute.WeaponType, "ammunition");
            type.Attributes.SetAttribute(ItemAttribute.BodyPosition, "ammo");
            type.Attributes.SetAttribute(ItemAttribute.Weight, 1);
            type.Flags.Add(ItemFlag.Stackable);

            return new Ammo(type, new Location(100, 100, 7), amount);
        }

        public static IPickupable CreateCoin(ushort id, byte amount, uint multiplier)
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.SetId(id);
            type.SetName("coin");
            type.Attributes.SetAttribute(ItemAttribute.Type, "coin");
            type.Attributes.SetAttribute(ItemAttribute.Worth, multiplier);
            type.Attributes.SetAttribute(ItemAttribute.Weight, 1);
            type.Flags.Add(ItemFlag.Stackable);

            return new Coin(type, new Location(100, 100, 7), amount);
        }

        public static IAttackRune CreateAttackRune(ushort id, DamageType damageType = DamageType.Energy,
            bool needTarget = true, ushort min = 100, ushort max = 100)
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.SetId(id);
            type.SetName("hmm");
            type.Attributes.SetAttribute(ItemAttribute.Damage, DamageTypeParser.Parse(damageType));
            type.Attributes.SetAttribute(ItemAttribute.NeedTarget, needTarget);
            type.Attributes.SetCustomAttribute("x", new[] {min.ToString(), max.ToString()});
            type.Attributes.SetCustomAttribute("y", new[] {min.ToString(), max.ToString()});

            return new AttackRune(type, new Location(100, 100, 7), 100);
        }

        public static IItem CreateTopItem(ushort id, byte topOrder)
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.SetId(id);
            type.SetName("item");

            if (topOrder == 1)
                type.SetFlag(ItemFlag.AlwaysOnTop);
            else
                type.SetFlag(ItemFlag.Bottom);

            return new Item(type, new Location(100, 100, 7));
        }
    }
}