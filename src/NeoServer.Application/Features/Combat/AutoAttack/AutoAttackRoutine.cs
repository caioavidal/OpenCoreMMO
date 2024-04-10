using Mediator;
using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Infrastructure.Thread;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Results;

namespace NeoServer.Application.Features.Combat.AutoAttack;

public class AutoAttackRoutine(IGameServer gameServer, ISender mediator)
{
    private IDictionary<uint, uint> AttackEventIds { get; } = new Dictionary<uint, uint>();
    public void Execute(ICombatActor actor)
    {
        if (actor is IMonster) return;
        
        AttackEventIds.TryGetValue(actor.CreatureId, out var attackEvent);
        if (attackEvent != 0) return;

        var result = Attack(actor);
        var attackSpeed = result ? actor.AttackSpeed : 300;

        AttackEventIds[actor.CreatureId] = gameServer.Scheduler.AddEvent(new SchedulerEvent((int)attackSpeed, () => Attack(actor)));
    }

    private bool Attack(ICombatActor actor)
    {
        var result = Result.NotPossible;
        AttackEventIds.TryGetValue(actor.CreatureId, out var attackEvent);

        if (actor.Attacking)
        {
            gameServer.CreatureManager.TryGetCreature(actor.AutoAttackTargetId, out var creature);

            result = creature is not ICombatActor enemy
                ? Result.NotPossible
                : mediator.Send(new AutoAttackCommand(actor, enemy)).Result;
        }
        else
        {
            if (attackEvent != 0)
            {
                gameServer.Scheduler.CancelEvent(attackEvent);
                attackEvent = 0;
                AttackEventIds[actor.CreatureId] = attackEvent;
            }
        }

        if (attackEvent == 0) return result.Succeeded;

        AttackEventIds[actor.CreatureId] = 0;
        Execute(actor);

        return result.Succeeded;
    }
}