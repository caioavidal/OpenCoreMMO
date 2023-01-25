using System;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.World.Algorithms.AStar;

internal static class AStarCondition
{
    public static bool Validate(IMap map, Location startPos, Location testPos,
        Location targetPos, ref int bestMatchDist, FindPathParams fpp)
    {
        if (!map.IsInRange(startPos, testPos, targetPos, fpp)) return false;

        if (fpp.ClearSight && !SightClear.IsSightClear(map, testPos, targetPos, true)) return false;

        var testDist = Math.Max(targetPos.GetSqmDistanceX(testPos), targetPos.GetSqmDistanceY(testPos));
        if (fpp.MaxTargetDist == 1)
        {
            if (testDist < fpp.MinTargetDist || testDist > fpp.MaxTargetDist) return false;

            return true;
        }

        if (testDist <= fpp.MaxTargetDist)
        {
            if (testDist < fpp.MinTargetDist) return false;

            if (testDist == fpp.MaxTargetDist)
            {
                bestMatchDist = 0;
                return true;
            }

            if (testDist > bestMatchDist)
            {
                //not quite what we want, but the best so far
                bestMatchDist = testDist;
                return true;
            }
        }

        return false;
    }
}