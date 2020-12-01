using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Server.Helpers.Extensions;
using System;
using System.Buffers;
using System.Collections.Generic;

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

        public bool Find(ICreature creature, Location target, FindPathParams fpp, out Direction[] directions)
        {
            var AStarTibia = new AStarTibia();

            directions = new Direction[0];

            if (fpp.OneStep)
            {
                return FindStep(creature, target, fpp, out directions);
            }

            if (fpp.MaxTargetDist > 1)
            {
                if (!FindPathToKeepDistance(creature, target, fpp, out directions))
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

        public bool FindPathToKeepDistance(ICreature creature, Location target, FindPathParams fpp, out Direction[] directions)
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

            if (xDistance < fpp.MaxTargetDist && xDistance >= yDistance)
            {
                directions = new Direction[1] { startLocation.DirectionTo(target) == Direction.West ? Direction.East : Direction.West };
                return true;
            }

            if (yDistance < fpp.MaxTargetDist && yDistance >= xDistance)
            {
                directions = new Direction[1] { startLocation.DirectionTo(target) == Direction.South ? Direction.North : Direction.South };
                return true;
            }

            return true;
        }
    }
}
