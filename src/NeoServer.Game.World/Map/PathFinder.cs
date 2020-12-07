using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Server.Helpers.Extensions;
using System;
using System.Buffers;
using System.Collections.Generic;
using NeoServer.Game.Contracts.World.Tiles;

namespace NeoServer.Game.World.Map
{
    public class PathFinder : IPathFinder
    {
        public PathFinder(IMap map) => Map = map;

        public IMap Map { get; set; }

        public bool Find(ICreature creature, Location target, out Direction[] directions)
        {
            var AStarTibia = new AStarTibia();
            return AStarTibia.GetPathMatching(Map, creature, target, new FindPathParams(true), out directions);
        }

        public bool Find(ICreature creature, Location target, FindPathParams fpp, ITileEnterRule tileEnterRule, out Direction[] directions)
        {
            var AStarTibia = new AStarTibia();

            directions = new Direction[0];

            if (creature.Location.Z != target.Z) return false;

            if (fpp.OneStep)
            {
                return FindStep(creature, target, fpp, out directions);
            }

            if (fpp.MaxTargetDist > 1)
            {
                if (!FindPathToKeepDistance(creature, target, fpp, tileEnterRule, out directions))
                {
                    return AStarTibia.GetPathMatching(Map, creature, target, fpp, out directions);
                }
                return true;
            }

            return AStarTibia.GetPathMatching(Map, creature, target, fpp, out directions);
        }

        public bool FindStep(ICreature creature, Location target, FindPathParams fpp, out Direction[] directions)
        {
            directions = new Direction[0];

            var startLocation = creature.Location;

            foreach (var neighbour in startLocation.Neighbours.Random())
            {
                if (neighbour.GetSqmDistanceX(target) > fpp.MaxTargetDist || neighbour.GetSqmDistanceY(target) > fpp.MaxTargetDist) continue;

                if (fpp.MaxTargetDist > 1 && (neighbour.GetSqmDistanceX(target) < fpp.MaxTargetDist && neighbour.GetSqmDistanceY(target) < fpp.MaxTargetDist)) continue;

                if (Map.CanWalkTo(neighbour, out var tile))
                {
                    directions = new Direction[1] { startLocation.DirectionTo(neighbour) };
                    return true;
                }
            }

            return false;
        }

        public bool FindPathToKeepDistance(ICreature creature, Location target, FindPathParams fpp, ITileEnterRule tileEnterRule, out Direction[] directions)
        {
            directions = new Direction[0];

            if (fpp.MaxTargetDist <= 1)
            {
                return true;
            }
            var startLocation = creature.Location;

            var xDistance = startLocation.GetSqmDistanceX(target);
            var yDistance = startLocation.GetSqmDistanceY(target);

            if (xDistance > fpp.MaxTargetDist || yDistance > fpp.MaxTargetDist)
            {
                return false;
            }
            if (xDistance == fpp.MaxTargetDist || yDistance == fpp.MaxTargetDist) return true;

            var currentDistance = startLocation.GetSumSqmDistance(target);
            foreach (var direction in new Direction[] { Direction.East, Direction.South, Direction.West, Direction.North })
            {

                var nextLocation = startLocation.GetNextLocation(direction);
                var nextDistance = nextLocation.GetSumSqmDistance(target);
                if (nextDistance / 2 >= fpp.MaxTargetDist)
                {
                    return true;
                }
                if (nextDistance < currentDistance) continue;

                if (!Map.CanGoToDirection(creature.Location, direction, tileEnterRule))
                {
                    continue;
                }
                directions = new Direction[] { direction };
                return true;
            }
            foreach (var direction in new Direction[] { Direction.West, Direction.North, Direction.East, Direction.South })
            {
                if (!Map.CanGoToDirection(creature.Location, direction, tileEnterRule))
                {
                    continue;
                }
                directions = new Direction[] { direction };
                return true;
            }

            return true;
        }
    }
}
