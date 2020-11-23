using NeoServer.Game.Common;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Players;
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
