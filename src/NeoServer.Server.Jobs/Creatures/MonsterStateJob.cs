using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Server.Jobs.Creatures
{
    public class MonsterStateJob
    {
        public static void Execute(IMonster monster)
        {
            if (monster.IsDead) return;

            monster.ChangeState();
            
            if (monster.State == MonsterState.LookingForEnemy)
            {
                monster.LookForNewEnemy();
            }

            if (monster.State == MonsterState.InCombat)
            {
                monster.MoveAroundEnemy();

                if (!monster.Attacking) {
                    monster.SelectTargetToAttack();
                    return;
                }

                if (monster.Metadata.TargetChance.Interval == 0) return;

                if (monster.Attacking && monster.Metadata.TargetChance.Chance < GameRandom.Random.Next(minValue: 1, maxValue: 100)) return;
                monster.SelectTargetToAttack();
            }

            if (monster.State == MonsterState.Sleeping)
            {
                monster.Sleep();
            }
            if(monster.State == MonsterState.Running)
            {
                monster.Escape();
            }
        }
    }
}