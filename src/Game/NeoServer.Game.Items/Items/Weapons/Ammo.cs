using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Bases;

namespace NeoServer.Game.Items.Items.Weapons
{
    public class Ammo : Cumulative, IAmmoItem
    {
        public Ammo(IItemType type, Location location, IDictionary<ItemAttribute, IConvertible> attributes) : base(
            type, location, attributes)
        {
        }

        public Ammo(IItemType type, Location location, byte amount) : base(type, location, amount)
        {
        }

        public ImmutableHashSet<VocationType> AllowedVocations { get; }

        public byte Range => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Range);

        public byte Attack => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Attack);

        public byte ExtraHitChance => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.HitChance);
        public AmmoType AmmoType => Metadata.AmmoType;
        public ShootType ShootType => Metadata.ShootType;
        public Tuple<DamageType, byte> ElementalDamage => Metadata.Attributes.GetWeaponElementDamage();
        public bool HasElementalDamage => ElementalDamage is not null;

        public ushort MinimumLevelRequired { get; }

        public ImmutableDictionary<SkillType, byte> SkillBonus { get; }

        public void Throw()
        {
            Reduce();
        }

        public static bool IsApplicable(IItemType type)
        {
            return type.WeaponType == WeaponType.Ammunition;
        }

        public void DressedIn(IPlayer player)
        {
            
        }

        public void UndressFrom(IPlayer player)
        {
        }
    }
}