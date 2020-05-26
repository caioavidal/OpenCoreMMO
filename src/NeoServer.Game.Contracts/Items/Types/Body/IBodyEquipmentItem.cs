using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Item;
using NeoServer.Game.Enums.Players;
using System.Collections.Immutable;

namespace NeoServer.Game.Contracts.Items.Types
{
    public interface IInventoryItem : IItem
    {
        public Slot Slot => Metadata.BodyPosition;
    }
    public interface IBodyEquipmentItem : IMoveableThing, IPickupable, IInventoryItem
    {
        bool Pickupable => true;
        ImmutableHashSet<VocationType> AllowedVocations { get; }
        ushort MinimumLevelRequired => Metadata.Attributes.GetAttribute<ushort>(ItemAttribute.MinimumLevel);
        public ImmutableDictionary<SkillType, byte> SkillBonus => Metadata.Attributes.SkillBonus.ToImmutableDictionary();
        public WeaponType WeaponType => Metadata.WeaponType;
    }
}
