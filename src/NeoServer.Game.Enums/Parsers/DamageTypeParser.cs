using NeoServer.Game.Common.Item;

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
        public static DamageType Parse(string type)
        {
            return type switch
            {
                "melee" => DamageType.Melee,
                "physical" => DamageType.Physical,
                "energy" => DamageType.Energy,
                "fire" => DamageType.Fire,
                "firefield" => DamageType.FireField,
                "manadrain" => DamageType.ManaDrain,
                "firearea" => DamageType.Fire,
                "poison" => DamageType.Earth,
                "earth" => DamageType.Earth,
                "bleed" => DamageType.Physical,
                "drown" => DamageType.Drown,
                "ice" => DamageType.Ice,
                "holy" => DamageType.Holy,
                "death" => DamageType.Death,
                "lifedrain" => DamageType.AbsorbPercentPhysical,
                "mortarea" => DamageType.Death,
                _ => DamageType.Melee
            };
        }
    }
}
