using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Model.Conditions;
using NeoServer.Game.Creatures.Spells;
using NeoServer.Game.Enums.Creatures.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Creatures
{
    public class HasteCondition : Condition
    {
        public HasteCondition(uint duration) : base(duration) { }

        public override ConditionType Type => ConditionType.Haste;
        
    }
}
