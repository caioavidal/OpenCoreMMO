using System.Collections.Immutable;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Bases;

namespace NeoServer.Game.Items.Items
{
    public class BodyDefenseEquipment : Equipment, IDefenseEquipment
    {
        public BodyDefenseEquipment(IItemType itemType, Location location)
            : base(itemType, location)
        {
            //todo damage protection
        }

        public ImmutableHashSet<VocationType> AllowedVocations { get; }

        public bool Pickupable => true;
        public ImmutableDictionary<DamageType, byte> DamageProtection { get; }

        public Slot Slot => Metadata.WeaponType == WeaponType.Shield ? Slot.Right : Metadata.BodyPosition;

        public static bool IsApplicable(IItemType type)
        {
            return type.BodyPosition switch
            {
                Slot.Body => true,
                Slot.Legs => true,
                Slot.Head => true,
                Slot.Feet => true,
                Slot.Right => true,
                Slot.Ring => true,
                Slot.Necklace => true,
                _ => false
            } || type.WeaponType == WeaponType.Shield;
        }
    }
}