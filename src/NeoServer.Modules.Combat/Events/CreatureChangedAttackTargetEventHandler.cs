using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Modules.Combat.AutoAttack;

namespace NeoServer.Modules.Combat.Events;

public class CreatureChangedAttackTargetEventHandler(AutoAttackRoutine autoAttackRoutine) : IEventHandler
{
    public void Execute(ICombatActor actor, uint oldTarget, uint newTarget)
    {
        autoAttackRoutine.Execute(actor);
    }
}