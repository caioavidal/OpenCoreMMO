using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Players;
using System.Collections.Immutable;

namespace NeoServer.Game.Items.Items
{
    public class BodyDefenseEquimentItem : MoveableItem, IDefenseEquipmentItem
    {
        public BodyDefenseEquimentItem(IItemType itemType, Location location)
            : base(itemType, location)
        {
            //todo damage protection
        }

        public bool Pickupable => true;

        public ImmutableHashSet<VocationType> AllowedVocations { get; }
        public ImmutableDictionary<DamageType, byte> DamageProtection { get; }

        public Slot Slot => Metadata.WeaponType == WeaponType.Shield ? Slot.Right : Metadata.BodyPosition;

        public VocationType[] Vocations => throw new System.NotImplementedException();

        public ushort MinLevel => throw new System.NotImplementedException();

        public static bool IsApplicable(IItemType type) =>
            type.BodyPosition switch
            {
                Slot.Body => true,
                Slot.Legs => true,
                Slot.Head => true,
                Slot.Feet => true,
                Slot.Right => true,
                _ => false
            } || type.WeaponType == WeaponType.Shield;
    }
}
