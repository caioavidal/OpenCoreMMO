using NeoServer.Game.Common.Creatures.Players;

namespace NeoServer.Game.Common.Conditions
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
