using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.Items.Types.Body;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Item;
using NeoServer.Game.Enums.Players;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace NeoServer.Game.Items.Items
{
    public class BodyDefenseEquimentItem : MoveableItem, IDefenseEquipmentItem
    {
        public BodyDefenseEquimentItem(IItemType itemType)
            : base(itemType)
        {

            DefenseValue = itemType.Attributes.GetAttribute<byte>(ItemAttribute.Defense);
            MinimumLevelRequired = itemType.Attributes.GetAttribute<ushort>(ItemAttribute.MinimumLevel);
            Weight = itemType.Attributes.GetAttribute<float>(ItemAttribute.Weight);
            SkillBonus = itemType.Attributes.SkillBonus?.ToImmutableDictionary();
            //todo damage protection
        }

        public bool Pickupable => true;
        public float Weight { get; }

        public ushort DefenseValue { get; }
        public ImmutableHashSet<VocationType> AllowedVocations { get; }
        public ushort MinimumLevelRequired { get; }
        public ImmutableDictionary<DamageType, byte> DamageProtection { get; }
        public ImmutableDictionary<SkillType, byte> SkillBonus { get; }

        public static bool IsApplicable(IItemType type) =>
            type.Attributes.GetAttribute(ItemAttribute.BodyPosition) switch
            {
                "body" => true,
                "legs" => true,
                "head" => true,
                "feet" => true,
                "shield" => true,
                _ => false
            };
    }
}
