using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Events.Combat
{
    public class CreatureChangedAttackTargetEventHandler
    {
        private readonly Game game;

        public CreatureChangedAttackTargetEventHandler(Game game)
        {
            this.game = game;
        }
        public void Execute(ICombatActor actor, uint oldTarget, uint newTarget)
        {
            
            if (actor.AttackEvent != 0)
            {
                return;
            }

            Attack(actor);

            actor.AttackEvent = game.Scheduler.AddEvent(new SchedulerEvent((int)actor.BaseAttackSpeed, () => Attack(actor)));
        }
        private void Attack(ICombatActor actor)
        {
            if (actor.Attacking)
            {
                if (game.CreatureManager.TryGetCreature(actor.AutoAttackTargetId, out var creature) && creature is ICombatActor enemy)
                {
                    actor.Attack(enemy);
                   // MoveAroundEnemy(actor);
                }
            }
            else
            {
                if (actor.AttackEvent != 0)
                {
                    game.Scheduler.CancelEvent(actor.AttackEvent);
                    actor.AttackEvent = 0;
                }
            }

            if (actor.AttackEvent != 0)
            {
                actor.AttackEvent = 0;
                Execute(actor, 0, 0);
            }
        }

        private void MoveAroundEnemy(ICombatActor actor)
        {
            if (!(actor is IMonster monster)) return;

            monster.MoveAroundEnemy();
        }
    }
}