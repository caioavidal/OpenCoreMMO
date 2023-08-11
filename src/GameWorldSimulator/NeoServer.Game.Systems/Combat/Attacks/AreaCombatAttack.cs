using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Systems.Events;

namespace NeoServer.Game.Systems.Combat.Attacks;

public static class AreaCombatAttack
{
    public static bool PropagateAttack(ICombatActor actor, CombatAttackParams combatAttackParams)
    {
        AreaCombatAttackProcessor.LastCombatAttackParamProcessed.Remove(actor.CreatureId);

        if (!combatAttackParams.Area.IsProcessed) AreaCombatAttackProcessor.Process(actor, combatAttackParams);

        CombatEvent.InvokeOnAttackingEvent(actor, new[] { combatAttackParams });

        if (combatAttackParams.Area.AffectedCreatures is null ||
            !combatAttackParams.Area.AffectedCreatures.Any()) return true;

        var attackResult = true;

        foreach (var victim in combatAttackParams.Area.AffectedCreatures)
        {
            foreach (var damage in combatAttackParams.Damages)
            {
                attackResult &= victim.ReceiveAttack(actor, damage);
            }
        }

        return attackResult;
    }
}