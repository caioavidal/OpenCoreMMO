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
            if (actor.AttackEvent != 0) return;

            var result = Attack(actor);
            var attackSpeed = result ? actor.BaseAttackSpeed : 300;
            actor.AttackEvent = game.Scheduler.AddEvent(new SchedulerEvent((int)attackSpeed, () => Attack(actor)));
        }
        private bool Attack(ICombatActor actor)
        {
            var result = false;
            if (actor.Attacking)
            {
                game.CreatureManager.TryGetCreature(actor.AutoAttackTargetId, out var creature);
                result = actor.Attack(creature);
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
            return result;
        }
    }
}