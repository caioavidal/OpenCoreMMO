using NeoServer.Game.Combat;
using NeoServer.Game.Combat.Validation;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Location;

namespace NeoServer.Game.Creatures.Monster.Combat;

public static class TargetDetector
{
    /// <summary>
    ///     Updates monster target list
    /// </summary>
    public static void UpdateTargets(Monster monster, IMapTool mapTool)
    {
        if (monster.Targets.IsNull()) return;

        var nearest = ushort.MaxValue;
        var nearestSightClear = ushort.MaxValue;

        monster.Targets.NearestTarget = null;
        monster.Targets.NearestSightClearTarget = null;

        foreach (CombatTarget target in monster.Targets)
        {
            target.ResetFlags();

            if (target.Creature.IsDead)
            {
                monster.Targets.RemoveTarget(target.Creature);
                continue;
            }

            if (mapTool.SightClearChecker.Invoke(monster.Location, target.Creature.Location) &&
                target.IsInRange(monster))
            {
                target.SetAsHasSightClear();

                var offsetSightClear = monster.Location.GetSqmDistance(target.Creature.Location);

                if (offsetSightClear >= nearestSightClear) continue;
                nearestSightClear = offsetSightClear;
                monster.Targets.NearestSightClearTarget = target;
            }

            var targetIsUnreachable = IsTargetUnreachable(monster, target, mapTool, out var directions);
            if (targetIsUnreachable) continue;

            target.SetAsReachable(directions);

            var offset = monster.Location.GetSqmDistance(target.Creature.Location);

            if (offset >= nearest) continue;

            nearest = offset;
            monster.Targets.NearestTarget = target;
        }
    }

    private static bool IsTargetUnreachable(Monster monster, CombatTarget target, IMapTool mapTool,
        out Direction[] directions)
    {
        if (mapTool.PathFinder.Find(monster, target.Creature.Location, monster.PathSearchParams, monster.TileEnterRule,
                out directions) == false) return true;

        if (AttackValidation.CanAttack(monster, target.Creature).Failed) return true;

        if (target.Creature.IsInvisible && !monster.CanSeeInvisible) return true;

        return false;
    }
}