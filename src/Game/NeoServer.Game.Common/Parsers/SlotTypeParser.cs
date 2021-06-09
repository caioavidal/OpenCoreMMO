using NeoServer.Game.Common.Players;
using NeoServer.Game.Contracts.Items;

namespace NeoServer.Game.Common.Parsers
{
    public class SlotTypeParser
    {
        public static Slot Parse(IItemAttributeList itemAttributes)
        {
            var slotType = itemAttributes?.GetAttribute(ItemAttribute.BodyPosition);

            if (slotType is null && itemAttributes.HasAttribute(ItemAttribute.WeaponType)) slotType = "weapon";

            return slotType switch
            {
                "body" => Slot.Body,
                "legs" => Slot.Legs,
                "head" => Slot.Head,
                "feet" => Slot.Feet,
                "shield" => Slot.Right,
                "ammo" => Slot.Ammo,
                "backpack" => Slot.Backpack,
                "ring" => Slot.Ring,
                "necklace" => Slot.Necklace,
                "two-handed" => Slot.TwoHanded,
                "weapon" => Slot.Left,
                _ => Slot.None
            };
        }
    }
}