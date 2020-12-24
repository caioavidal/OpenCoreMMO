using NeoServer.Game.Common.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Common.Parsers
{
    public class DamageTypeParser
    {
        public static string Parse(DamageType type)
        {
            return type switch
            {
                DamageType.Melee => "melee",
                DamageType.Physical => "physical",
                DamageType.Energy => "energy",
                DamageType.Fire => "fire",
                DamageType.FireField => "fire",
                DamageType.ManaDrain => "manadrain",
                DamageType.Earth => "earth",
                DamageType.Drown => "drown",
                DamageType.Ice => "ice",
                DamageType.Holy => "holy",
                DamageType.Death => "death",
                DamageType.AbsorbPercentPhysical => "lifedrain",
                _ => "physical"
            };
        }
    }
}
