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
            if (monster.IsDead || monster.IsSleeping)
            {
                return;
            }
            if (!monster.IsInCombat)
            {
                return;
            }
            if (monster.Metadata.TargetChance.Interval == 0)
            {
                return;
            }

            if (monster.Metadata.TargetChance.Chance < ServerRandom.Random.Next(minValue: 1, maxValue: 100))
            {
                return;
            }

            monster.SelectTargetToAttack();
        }
    }
}
