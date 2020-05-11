using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Item;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Enums.Players;
using System;
using System.Collections.Immutable;

namespace NeoServer.Game.Items.Items
{
    public class WeaponItem : MoveableItem, IWeaponItem
    {
        public WeaponItem(IItemType itemType, Location location) : base(itemType, location)
        {
            Attack = itemType.Attributes.GetAttribute<ushort>(ItemAttribute.Attack);
            Defense = itemType.Attributes.GetAttribute<byte>(ItemAttribute.Defense);
            ElementalDamage = itemType.Attributes.GetWeaponElementDamage();
            ExtraDefense = itemType.Attributes.GetAttribute<sbyte>(ItemAttribute.ExtraDefense);
            TwoHanded = itemType.Attributes.GetAttribute(ItemAttribute.BodyPosition) == "two-handed";
            MinimumLevelRequired = itemType.Attributes.GetAttribute<ushort>(ItemAttribute.MinimumLevel);
            Weight = itemType.Attributes.GetAttribute<float>(ItemAttribute.Weight);
            //AllowedVocations  todo
        }

        public ushort Attack { get; }

        public byte Defense { get; }

        public Tuple<DamageType, byte> ElementalDamage { get; }

        public sbyte ExtraDefense { get; }

        public bool TwoHanded { get; }

        public ImmutableHashSet<VocationType> AllowedVocations { get; }

        public ushort MinimumLevelRequired { get; }

        public ImmutableDictionary<SkillType, byte> SkillBonus { get; }

        public float Weight { get; }

        public static bool IsApplicable(IItemType type) =>
            type.Attributes.GetAttribute(ItemAttribute.WeaponType) switch
            {
                "axe" => true,
                "sword" => true,
                "club" => true,
                _ => false
            };

    }
}
