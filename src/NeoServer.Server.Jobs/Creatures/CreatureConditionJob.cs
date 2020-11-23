using NeoServer.Game.Common.Conditions;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Jobs.Creatures
{
    public class CreatureConditionJob
    {
        public static void Execute(ICombatActor creature)
        {
            if (creature.IsDead)
            {
                return;
            }

            foreach (var condition in creature.Conditions)
            {
                if (condition.Value.HasExpired)
                {
                    condition.Value.End();
                    creature.RemoveCondition(condition.Value);
                }
                if(condition.Value is DamageCondition damageCondition)
                {
                    damageCondition.Execute(creature);
                }
            }
        }
    }
}
