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
            _ => VocationType.All
        };
        public static string Parse(VocationType vocation) => vocation switch
        {
            VocationType.Paladin => "Paladin",
            VocationType.Knight => "Knight",
            VocationType.Sorcerer => "Sorcerer",
            VocationType.Druid => "Druid",
            VocationType.All => "All",
            VocationType.Noob => "Noob",
            _ => "All"
        };
    }
}
