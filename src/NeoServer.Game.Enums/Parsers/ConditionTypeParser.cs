using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Parsers
{
    public class ConditionTypeParser
    {
        public static DamageType Parse(ConditionType type)
        {
            return type switch
            {
                ConditionType.Poison => DamageType.Earth,
                ConditionType.Fire => DamageType.FireField,
                ConditionType.Energy => DamageType.Energy,
                _ => DamageType.None
            };
        }
        public static ConditionType Parse(DamageType type)
        {
            return type switch
            {
                DamageType.Earth => ConditionType.Poison,
                DamageType.FireField => ConditionType.Fire,
                DamageType.Fire => ConditionType.Fire,
                DamageType.Energy => ConditionType.Energy,
                _ => ConditionType.None
            };
        }
    }
}
