using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.Items.Types.Body;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Enums.Players;
using System.Collections.Immutable;

namespace NeoServer.Game.Items.Items
{
    public class DistanceWeaponItem : MoveableItem, IWeapon
    {
        public DistanceWeaponItem(IItemType type, Location location) : base(type, location)
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


        public ImmutableHashSet<VocationType> AllowedVocations { get; }

        public static bool IsApplicable(IItemType type) => type.Attributes.GetAttribute(Enums.ItemAttribute.WeaponType) == "distance";
    }
}
