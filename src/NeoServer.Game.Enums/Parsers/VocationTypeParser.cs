using NeoServer.Game.Common.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Common.Parsers
{
    public class VocationTypeParser
    {
        public static VocationType Parse(string vocation) => vocation switch
        {
            "Paladin" => VocationType.Paladin,
            "Knight" => VocationType.Knight,
            "Sorcerer" => VocationType.Sorcerer,
            "Druid" => VocationType.Druid,
            "All" => VocationType.All,
            "None" => VocationType.None,
            "Master Sorcerer" => VocationType.MasterSorcerer,
            "Elder Druid" => VocationType.ElderDruid,
            "Royal Paladin" => VocationType.RoyalPaladin,
            "Elite Knight" => VocationType.EliteKnight,
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
