using NeoServer.Game.Common.Players;

namespace NeoServer.Game.Common.Parsers
{
    public class VocationTypeParser
    {
        public static VocationType Parse(string vocation) => vocation.ToLower().Replace(" ","") switch
        {
            "paladin" => VocationType.Paladin,
            "knight" => VocationType.Knight,
            "sorcerer" => VocationType.Sorcerer,
            "druid" => VocationType.Druid,
            "all" => VocationType.All,
            "none" => VocationType.None,
            "mastersorcerer" => VocationType.MasterSorcerer,
            "elderdruid" => VocationType.ElderDruid,
            "royalpaladin" => VocationType.RoyalPaladin,
            "eliteknight" => VocationType.EliteKnight,
            _ => VocationType.All
        };
        public static string Parse(VocationType vocation) => vocation switch
        {
            VocationType.Paladin => "Paladin",
            VocationType.Knight => "Knight",
            VocationType.Sorcerer => "Sorcerer",
            VocationType.Druid => "Druid",
            VocationType.RoyalPaladin => "Royal Paladin",
            VocationType.EliteKnight => "Elite Knight",
            VocationType.MasterSorcerer => "Master Sorcerer",
            VocationType.ElderDruid => "Elder Druid",
            VocationType.All => "All",
            VocationType.None => "",
            _ => "All"
        };
    }
}
