using NeoServer.Game.Combat;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Creatures.Monster.Actions;

internal static class MonsterEscape
{
    public static void Escape(Monster monster)
    {
        monster.StopFollowing();
        monster.StopAttack();

        ICreature escapeFrom = null;

        var targets = monster.Targets;

        if (targets.TryGetTarget(monster.AutoAttackTargetId, out var creature))
        {
            escapeFrom = creature.Creature;
        }
        else
            foreach (CombatTarget target in targets)
            {
                if (target.CanReachCreature)
                {
                    escapeFrom = target.Creature;
                    break;
                }

                escapeFrom = target.Creature;
            }

        if (escapeFrom is null) return;

        monster.Escape(escapeFrom.Location);
    }

}