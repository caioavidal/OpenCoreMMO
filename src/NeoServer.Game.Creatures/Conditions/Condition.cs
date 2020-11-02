using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Model.Conditions;
using NeoServer.Game.Creatures.Spells;
using NeoServer.Game.Enums.Creatures.Players;
using System;
using System.Collections.Generic;
using System.Text;

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
