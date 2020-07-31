using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Body;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Item;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Enums.Players;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace NeoServer.Game.Items.Items
{
    public class AmmoItem : CumulativeItem, IAmmoItem
    {
        public AmmoItem(IItemType type, Location location, IDictionary<ItemAttribute, IConvertible> attributes) : base(type, location, attributes)
        {
            Range = type.Attributes.GetAttribute<byte>(Enums.ItemAttribute.Range);
            Attack = type.Attributes.GetAttribute<byte>(Enums.ItemAttribute.Attack);
            ExtraHitChance = type.Attributes.GetAttribute<byte>(Enums.ItemAttribute.HitChance);
            //AllowedVocations = allowedVocations;
            //MinimumLevelRequired = minimumLevelRequired;
            //SkillBonus = skillBonus;
        }
        //todo duplicated code
        public AmmoItem(IItemType type, Location location, byte amount) : base(type, location, amount)
        {
            Range = type.Attributes.GetAttribute<byte>(Enums.ItemAttribute.Range);
            Attack = type.Attributes.GetAttribute<byte>(Enums.ItemAttribute.Attack);
            ExtraHitChance = type.Attributes.GetAttribute<byte>(Enums.ItemAttribute.HitChance);
            //AllowedVocations = allowedVocations;
            //MinimumLevelRequired = minimumLevelRequired;
            //SkillBonus = skillBonus;
        }

        public byte Range { get; }

        public byte Attack { get; }

        public byte ExtraHitChance { get; }
        public AmmoType AmmoType => Metadata.AmmoType;
        public ShootType ShootType => Metadata.ShootType;

        public ImmutableHashSet<VocationType> AllowedVocations { get; }

        public ushort MinimumLevelRequired { get; }

        public ImmutableDictionary<SkillType, byte> SkillBonus { get; }


        public static new bool IsApplicable(IItemType type) => type.WeaponType ==  WeaponType.Ammunition;
    }
}
