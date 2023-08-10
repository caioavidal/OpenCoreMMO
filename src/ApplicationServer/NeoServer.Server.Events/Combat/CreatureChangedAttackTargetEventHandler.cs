using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Results;
using NeoServer.Game.Systems.Combat.Systems;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Events.Combat;

public class CreatureChangedAttackTargetEventHandler
{
    private readonly IGameServer game;
    private readonly AutoAttackSystem _autoAttackSystem;

    public CreatureChangedAttackTargetEventHandler(IGameServer game, AutoAttackSystem autoAttackSystem)
    {
        this.game = game;
        _autoAttackSystem = autoAttackSystem;
    }

    public void Execute(ICombatActor actor, uint oldTarget, uint newTarget)
    {
        if (actor.AttackEvent != 0) return;

        var result = Attack(actor);
        var attackSpeed = result ? actor.AttackSpeed : 300;
        actor.AttackEvent = game.Scheduler.AddEvent(new SchedulerEvent((int)attackSpeed, () => Attack(actor)));
    }

    private bool Attack(ICombatActor actor)
    {
        var result = Result.NotPossible;

        if (actor.Attacking)
        {
            game.CreatureManager.TryGetCreature(actor.AutoAttackTargetId, out var creature);

            result = creature is not ICombatActor enemy ? Result.NotPossible : _autoAttackSystem.RunCombatTurn(actor, enemy);
        }
        else
        {
            if (actor.AttackEvent != 0)
            {
                game.Scheduler.CancelEvent(actor.AttackEvent);
                actor.AttackEvent = 0;
            }
        }

        if (actor.AttackEvent == 0) return result.Succeeded;

        actor.AttackEvent = 0;
        Execute(actor, 0, 0);

        return result.Succeeded;
    }
}