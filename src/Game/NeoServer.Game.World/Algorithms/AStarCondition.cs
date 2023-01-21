using System;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.World.Algorithms;

public struct AStarCondition
{
    public bool Validate(IMap map, Location startPos, Location testPos,
        Location targetPos, ref int bestMatchDist, FindPathParams fpp)
    {
        if (!map.IsInRange(startPos, testPos, targetPos, fpp)) return false;

        if (fpp.ClearSight && !SightClear.IsSightClear(map, testPos, targetPos, true)) return false;

        var testDist = Math.Max(targetPos.GetSqmDistanceX(testPos), targetPos.GetSqmDistanceY(testPos));
        
        if (fpp.MaxTargetDist == 1)
        {
            return testDist >= fpp.MinTargetDist && testDist <= fpp.MaxTargetDist;
        }

        if (testDist > fpp.MaxTargetDist) return false;
        if (testDist < fpp.MinTargetDist) return false;
        if (testDist <= bestMatchDist) return false;
        
        bestMatchDist = testDist != fpp.MaxTargetDist ? 0 : testDist;

        return true;
    }
}