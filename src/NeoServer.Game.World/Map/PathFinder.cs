using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;

namespace NeoServer.Game.World.Map
{
    public class PathFinder : IPathFinder
    {
        public PathFinder(IMap map) => Map = map;

        public IMap Map { get; set; }

        public bool Find(ICreature creature, Location target, ITileEnterRule tileEnterRule, out Direction[] directions)
        {
            var AStarTibia = new AStarTibia();
            return AStarTibia.GetPathMatching(Map, creature, target, new FindPathParams(true), tileEnterRule, out directions);
        }

        public bool Find(ICreature creature, Location target, FindPathParams fpp, ITileEnterRule tileEnterRule, out Direction[] directions)
        {
            var AStarTibia = new AStarTibia();

            directions = new Direction[0];

            if (creature.Location.Z != target.Z) return false;

            if (fpp.OneStep)
            {
                return FindStep(creature, target, fpp, tileEnterRule, out directions);
            }

            if (fpp.MaxTargetDist > 1)
            {
                if (!FindPathToKeepDistance(creature, target, fpp, tileEnterRule, out directions))
                {
                    return AStarTibia.GetPathMatching(Map, creature, target, fpp, tileEnterRule, out directions);
                }
                return true;
            }

            return AStarTibia.GetPathMatching(Map, creature, target, fpp, tileEnterRule, out directions);
        }

        public bool FindStep(ICreature creature, Location target, FindPathParams fpp, ITileEnterRule tileEnterRule, out Direction[] directions)
        {
            directions = new Direction[0];

            var startLocation = creature.Location;

            var possibleDirections = new Direction[] { Direction.East, Direction.South, Direction.West, Direction.North, Direction.NorthEast };

            foreach (var direction in possibleDirections)
            {
                if (startLocation.GetMaxSqmDistance(target) > fpp.MaxTargetDist) continue;
                
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

            var currentDistance = startLocation.GetMaxSqmDistance(target);

            if (currentDistance > fpp.MaxTargetDist) return false;

            if (currentDistance == fpp.MaxTargetDist) return true;

            var possibleDirections = new Direction[] { Direction.East, Direction.South, Direction.West, Direction.North, Direction.NorthEast, Direction.NorthEast, Direction.SouthEast, Direction.SouthWest };

            (Direction, int) directionWeight = (Direction.None, 0);

            var canGoToDirections = new Direction[8];
            var canGoIndex = 0;

            var cannotGoDirectionCount = 0;
            foreach (var direction in possibleDirections)
            {
                var nextLocation = startLocation.GetNextLocation(direction);
                var nextDistance = nextLocation.GetMaxSqmDistance(target);

                var canGoToDirection = Map.CanGoToDirection(creature.Location, direction, tileEnterRule);
                var isDiagonalMovement = startLocation.IsDiagonalMovement(nextLocation);

                if (!canGoToDirection)
                {
                    if (!isDiagonalMovement) cannotGoDirectionCount++;
                    continue;
                }

                var weight = 0;

                if (isDiagonalMovement && cannotGoDirectionCount < 4)
                    continue;

                canGoToDirections[canGoIndex++] = direction;

                if (nextDistance > currentDistance)
                {
                    weight++;
                }
                if (nextLocation.GetSumSqmDistance(target) > startLocation.GetSumSqmDistance(target))
                {
                    weight++;
                }

                if (weight > directionWeight.Item2)
                {
                    directionWeight.Item1 = direction;
                    directionWeight.Item2 = weight;
                }
            }

            if (directionWeight.Item1 != Direction.None)
            {
                directions = new Direction[] { directionWeight.Item1 };
                return true;
            }

            if (canGoIndex > 0)
            {
                var randonIndex = GameRandom.Random.Next(minValue: 0, maxValue: canGoIndex);
                directions = new Direction[] { canGoToDirections[randonIndex] };
                return true;
            }

            return true;
        }
    }
}
