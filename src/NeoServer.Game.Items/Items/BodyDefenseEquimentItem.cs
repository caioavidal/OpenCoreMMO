using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Body;
using NeoServer.Game.Enums.Item;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Enums.Players;
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
