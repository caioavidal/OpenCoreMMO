using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using System;
using System.Buffers;
using System.Collections.Generic;

namespace NeoServer.Game.World.Map
{
    public class PathFinder : IPathFinder
    {
        public PathFinder(IMap map)
        {
            Map = map;
        }

        public IMap Map { get; set; }

        public bool Find(ICreature creature, Location target, out Direction[] directions)
        {
            var AStarTibia = new AStarTibia();
            return AStarTibia.GetPathMatching(Map, creature, target, new FindPathParams(true), out directions);
        }

        public bool Find(ICreature creature, Location target, FindPathParams fpp, out Direction[] directions)
        {
            var AStarTibia = new AStarTibia();

            if (fpp.KeepDistance)
            {
                FindPathToKeepDistance(creature, target, fpp, out directions);

                if (creature.Location.GetSqmDistanceX(target) > fpp.MaxTargetDist || creature.Location.GetSqmDistanceY(target) > fpp.MaxTargetDist)
                {
                    return AStarTibia.GetPathMatching(Map, creature, target, new FindPathParams(fpp.FullPathSearch, fpp.ClearSight, fpp.AllowDiagonal, false, fpp.MaxSearchDist, 1, fpp.MaxTargetDist), out directions);
                }
                return true;
            }

            return AStarTibia.GetPathMatching(Map, creature, target, fpp, out directions);

        }

        public void FindPathToKeepDistance(ICreature creature, Location target, FindPathParams fpp, out Direction[] directions)
        {
            directions = new Direction[0];

            if (fpp.KeepDistance == false)
            {
                return;
            }
            var startLocation = creature.Location;

            var xDistance = startLocation.GetSqmDistanceX(target);
            var yDistance = startLocation.GetSqmDistanceY(target);

            if (xDistance >= fpp.MaxTargetDist || yDistance >= fpp.MaxTargetDist)
            {
                return;
            }

            if (xDistance < fpp.MaxTargetDist && xDistance >= yDistance)
            {
                directions = new Direction[1] { startLocation.DirectionTo(target) == Direction.West ? Direction.East : Direction.West };
                return;
            }
            if (yDistance < fpp.MaxTargetDist && yDistance > xDistance)
            {
                directions = new Direction[1] { startLocation.DirectionTo(target) == Direction.South ? Direction.North : Direction.South };
                return;
            }
        }
    }
}
