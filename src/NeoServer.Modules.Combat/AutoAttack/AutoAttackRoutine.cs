using Mediator;
using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.BuildingBlocks.Infrastructure.Threading.Scheduler;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Results;
using NeoServer.Modules.Combat.PlayerAttack;
using NeoServer.Modules.Combat.PlayerAttack.WeaponAttack;

namespace NeoServer.Modules.Combat.AutoAttack;

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

        AttackEventIds[actor.CreatureId] =
            gameServer.Scheduler.AddEvent(new SchedulerEvent((int)attackSpeed, () => Attack(actor)));
    }

    private bool Attack(ICombatActor actor)
    {
        var result = Result.NotPossible;
        AttackEventIds.TryGetValue(actor.CreatureId, out var attackEvent);

        if (actor.Attacking)
        {
            result = AttackIfAggressorIsPlayer(actor);
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

    private Result AttackIfAggressorIsPlayer(ICombatActor aggressor)
    {
        if (aggressor is not IPlayer player) return Result.NotApplicable;

        gameServer.CreatureManager.TryGetCreature(aggressor.AutoAttackTargetId, out var creature);
        if (player.CurrentTarget is null) return Result.NotPossible;

        //this is for external callback. ie: lua
        if (IWeapon.AttackFunction?.Invoke(player, player.Inventory.Weapon) is true) return Result.Success;

        var attackParameter = PlayerAttackParameterBuilder.Build(player);
        var command = new PlayerWeaponAttackCommand(player, player.CurrentTarget, attackParameter);

        return mediator.Send(command).Result;
    }
}