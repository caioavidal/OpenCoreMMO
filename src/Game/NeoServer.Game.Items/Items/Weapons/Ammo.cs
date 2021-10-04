using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Parsers;
using NeoServer.Game.Items.Bases;
using NeoServer.Game.Items.Items.Cumulatives;

namespace NeoServer.Game.Items.Items.Weapons
{
    public class Ammo : CumulativeEquipment, IAmmoEquipment
    {
        public Ammo(IItemType type, Location location, IDictionary<ItemAttribute, IConvertible> attributes) : base(type,
            location, attributes)
        {
        }

        public Ammo(IItemType type, Location location, byte amount) : base(type, location, amount)
        {
        }

        public byte Range => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Range);

        public byte Attack => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Attack);

        public override bool CanBeDressed(IPlayer player)
        {
            if (Guard.IsNullOrEmpty(Vocations)) return true;

            foreach (var vocation in Vocations)
            {
                if (vocation == player.VocationType) return true;
            }

            return false;
        }

        public byte ExtraHitChance => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.HitChance);
        public AmmoType AmmoType => Metadata.AmmoType;
        public ShootType ShootType => Metadata.ShootType;
        public Tuple<DamageType, byte> ElementalDamage => Metadata.Attributes.GetWeaponElementDamage();
        public bool HasElementalDamage => ElementalDamage is not null;

        public ushort MinimumLevelRequired { get; }

        public ImmutableDictionary<SkillType, byte> SkillBonus { get; }

        protected override string PartialInspectionText
        {
            get
            {
                var elementalDamageText = ElementalDamage is not null && ElementalDamage.Item2 > 0
                    ? $" + {ElementalDamage.Item2} {DamageTypeParser.Parse(ElementalDamage.Item1)}"
                    : string.Empty;

                return $"Atk: {Attack}{elementalDamageText}";
            }
        }

        public void Throw()
        {
            Reduce();
        }

        public static bool IsApplicable(IItemType type)
        {
            return type.WeaponType == WeaponType.Ammunition;
        }
        
    }
}