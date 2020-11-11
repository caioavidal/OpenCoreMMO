using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Server.Helpers;
using NeoServer.Server.Tasks;
using System;

namespace NeoServer.Server.Jobs.Creatures
{
    public class CreatureTargetJob
    {        
        public static void Execute(ICombatActor creature)
        {
          
            if (!(creature is IMonster monster))
            {
                return;
            }
            if (monster.IsDead)
            {
                return;
            }
            if (!monster.IsInCombat)
            {
                return;
            }
            if (monster.Metadata.TargetChange.Interval == 0)
            {
                return;
            }

            if (monster.Metadata.TargetChange.Chance < GaussianRandom.Random.Next(minValue: 1, maxValue: 100))
            {
                return;
            }

            monster.ForgetTargets();
            monster.SelectTargetToAttack();
        }
    }
}
