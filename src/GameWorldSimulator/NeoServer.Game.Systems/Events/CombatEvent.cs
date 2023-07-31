using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Systems.Events;

public static class CombatEvent
{
    public static event Attack OnCreatureAttacking;

    public static void InvokeOnAttackingEvent(ICombatActor creature, ICreature victim,
        CombatAttackParams[] combatAttacks) => OnCreatureAttacking?.Invoke(creature, victim, combatAttacks);

    public static void InvokeOnAttackingEvent(ICombatActor creature, CombatAttackParams[] combatAttacks)
    {
        OnCreatureAttacking?.Invoke(creature, null, combatAttacks);
    }
}