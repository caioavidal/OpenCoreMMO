using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Features.Combat.AutoAttack;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Application.Features.Combat.Events;

public class CreatureChangedAttackTargetEventHandler(AutoAttackRoutine autoAttackRoutine) : IEventHandler
{
    public void Execute(ICombatActor actor, uint oldTarget, uint newTarget) => autoAttackRoutine.Execute(actor);
}