using NeoServer.Game.Creatures.Model.Conditions;
using NeoServer.Game.Enums.Creatures.Players;

namespace NeoServer.Game.Creatures
{
    public class Condition : BaseCondition
    {
        public Condition(ConditionType type, uint duration):base(duration)
        {
            Type = type;
        }
        public override ConditionType Type { get; }
    }

}
