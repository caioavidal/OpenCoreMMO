using NeoServer.Game.Common.Players;

namespace NeoServer.Game.Common.Parsers
{
    public class SlotTypeParser
    {
        public static Slot Parse(string slotType) => slotType switch
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
            _ => Slot.WhereEver
        };
    }
}
