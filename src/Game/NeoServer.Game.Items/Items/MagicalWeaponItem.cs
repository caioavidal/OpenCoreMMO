using NeoServer.Game.Common;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Players;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Body;
using System.Collections.Immutable;

namespace NeoServer.Game.Items.Items
{
    public class MagicalWeaponItem : MoveableItem, IMagicalWeaponItem
    {
        public MagicalWeaponItem(IItemType itemType, Location location) : base(itemType, location)
        {
            //DamageType = itemType.GetAttribute<ushort>(ItemAttribute.Attack);
            //Defense = itemType.GetAttribute<byte>(ItemAttribute.Defense);
            //ElementalDamage = itemType.GetWeaponElementDamage();
            //ExtraDefense = itemType.GetAttribute<sbyte>(ItemAttribute.ExtraDefense);
            //TwoHanded = itemType.GetAttribute(ItemAttribute.BodyPosition) == "two-handed";
            //MinimumLevelRequired = itemType.GetAttribute<ushort>(ItemAttribute.MinimumLevel);
            //Weight = itemType.GetAttribute<float>(ItemAttribute.Weight);
        }

        public DamageType DamageType { get; }

        public byte AverageDamage { get; }

        public byte ManaWasting { get; }

        public byte Range { get; }

        public ImmutableHashSet<VocationType> AllowedVocations { get; }

        public ushort MinimumLevelRequired { get; }

        public ImmutableDictionary<SkillType, byte> SkillBonus { get; }

        public static bool IsApplicable(IItemType type) => type.Attributes.GetAttribute(ItemAttribute.WeaponType) == "wand";

    }
}
