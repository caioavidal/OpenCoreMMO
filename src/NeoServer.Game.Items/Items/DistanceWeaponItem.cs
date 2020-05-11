using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Body;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Enums.Players;
using System.Collections.Immutable;

namespace NeoServer.Game.Items.Items
{
    public class DistanceWeaponItem : MoveableItem, IDistanceWeaponItem
    {
        public DistanceWeaponItem(IItemType type, Location location) : base(type, location)
        {
            MaxAttackDistance = type.Attributes.GetAttribute<byte>(Enums.ItemAttribute.Range);
            Attack = type.Attributes.GetAttribute<byte>(Enums.ItemAttribute.Attack);
            ExtraHitChance = type.Attributes.GetAttribute<byte>(Enums.ItemAttribute.HitChance);
            TwoHanded = type.Attributes.GetAttribute(Enums.ItemAttribute.BodyPosition) == "two-handed";
            //AllowedVocations = allowedVocations;
            //MinimumLevelRequired = minimumLevelRequired;
            //SkillBonus = skillBonus;
            Weight = type.Attributes.GetAttribute<float>(Enums.ItemAttribute.Weight);
        }

        public byte MaxAttackDistance { get; }

        public byte Attack { get; }

        public byte ExtraHitChance { get; }

        public bool TwoHanded { get; }

        public ImmutableHashSet<VocationType> AllowedVocations { get; }

        public ushort MinimumLevelRequired { get; }

        public ImmutableDictionary<SkillType, byte> SkillBonus { get; }

        public float Weight { get; }

        public static bool IsApplicable(IItemType type) => type.Attributes.GetAttribute(Enums.ItemAttribute.WeaponType) == "distance";
    }
}
