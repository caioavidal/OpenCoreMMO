using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Helpers;
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
        }

        protected override string PartialInspectionText
        {
            get
            {
                var hasArmorValue = Metadata.Attributes.TryGetAttribute<byte>(ItemAttribute.ArmorValue, out var armorValue);
                if (hasArmorValue) return $"Arm: {armorValue}";
                
                var hasDefenseValue = Metadata.Attributes.TryGetAttribute<byte>(ItemAttribute.Defense, out var defenseValue);
                return hasDefenseValue ? $"Def: {defenseValue}" : string.Empty;
            }
        }
        public bool Pickupable => true;

        public Slot Slot => Metadata.WeaponType == WeaponType.Shield ? Slot.Right : Metadata.BodyPosition;

        public static bool IsApplicable(IItemType type)
        {
            if (Guard.IsNull(type)) return false;
            
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