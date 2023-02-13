using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Parsers;

public class SlotTypeParser
{
    public static Slot Parse(IItemAttributeList itemAttributes)
    {
        var slotType = itemAttributes?.GetAttribute(ItemAttribute.BodyPosition);

        if (itemAttributes != null && slotType is null && itemAttributes.HasAttribute(ItemAttribute.WeaponType))
            slotType = "weapon";

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