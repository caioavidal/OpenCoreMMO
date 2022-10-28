using System;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.World.Algorithms;

namespace NeoServer.Game.World.Map;

public class PathFinder : IPathFinder
{
    public PathFinder(IMap map)
    {
        Map = map;
    }

    public IMap Map { get; set; }

    public bool Find(ICreature creature, Location target, ITileEnterRule tileEnterRule, out Direction[] directions)
    {
        var AStarTibia = new AStarTibia();
        return AStarTibia.GetPathMatching(Map, creature, target, new FindPathParams(true), tileEnterRule,
            out directions);
    }

    public bool Find(ICreature creature, Location target, FindPathParams fpp, ITileEnterRule tileEnterRule,
        out Direction[] directions)
    {
        directions = Array.Empty<Direction>();

        if (creature is not IWalkableCreature walkableCreature) return false;

        if (walkableCreature.Speed == 0) return false;

        if (!creature.Location.SameFloorAs(target)) return false;

        if (!fpp.KeepDistance && creature.Location.IsNextTo(target)) return true;

        if (fpp.OneStep) return FindStep(creature, target, fpp, tileEnterRule, out directions);

        var aStarTibia = new AStarTibia();

        if (fpp.MaxTargetDist > 1)
        {
            if (!FindPathToKeepDistance(creature, target, fpp, tileEnterRule, out directions))
                return aStarTibia.GetPathMatching(Map, creature, target, fpp, tileEnterRule, out directions);
            return true;
        }

        return aStarTibia.GetPathMatching(Map, creature, target, fpp, tileEnterRule, out directions);
    }
    
    
    public Direction FindRandomStep(ICreature creature, ITileEnterRule rule, Location origin, int maxStepsFromOrigin = 1)
    {
        var randomIndex = GameRandom.Random.Next(0, maxValue: 4);

        var directions = new[] { Direction.East, Direction.North, Direction.South, Direction.West };

        for (var i = 0; i < 4; i++)
        {
            randomIndex = randomIndex > 3 ? 0 : randomIndex;
            var direction = directions[randomIndex++];

            if (Map.CanGoToDirection(creature, direction, rule))
            {
                var nextLocation = creature.Location.GetNextLocation(direction);
                if (nextLocation.GetMaxSqmDistance(origin) > maxStepsFromOrigin) continue;
                
                return direction;
            }
        }

        return Direction.None;
    }

    public Direction FindRandomStep(ICreature creature, ITileEnterRule rule)
    {
        var randomIndex = GameRandom.Random.Next(0, maxValue: 4);

        var directions = new Direction[4] { Direction.East, Direction.North, Direction.South, Direction.West };

        for (var i = 0; i < 4; i++)
        {
            randomIndex = randomIndex > 3 ? 0 : randomIndex;
            var direction = directions[randomIndex++];

            if (Map.CanGoToDirection(creature, direction, rule)) return direction;
        }

        return Direction.None;
    }

    public bool FindStep(ICreature creature, Location target, FindPathParams fpp, ITileEnterRule tileEnterRule,
        out Direction[] directions)
    {
        directions = new Direction[0];

        var startLocation = creature.Location;

        var possibleDirections = new[]
            { Direction.East, Direction.South, Direction.West, Direction.North, Direction.NorthEast };

        foreach (var direction in possibleDirections)
            if (startLocation.GetMaxSqmDistance(target) > fpp.MaxTargetDist)
                continue;

        return false;
    }

    public bool FindPathToKeepDistance(ICreature creature, Location target, FindPathParams fpp,
        ITileEnterRule tileEnterRule, out Direction[] directions)
    {
        directions = new Direction[0];

        if (fpp.MaxTargetDist <= 1) return true;
        var startLocation = creature.Location;

        var currentDistance = startLocation.GetMaxSqmDistance(target);

        if (currentDistance > fpp.MaxTargetDist) return false;

        if (currentDistance == fpp.MaxTargetDist) return true;

        var possibleDirections = new[]
        {
            Direction.East, Direction.South, Direction.West, Direction.North, Direction.NorthEast,
            Direction.NorthEast, Direction.SouthEast, Direction.SouthWest
        };

        (Direction, int) directionWeight = (Direction.None, 0);

        var canGoToDirections = new Direction[8];
        var canGoIndex = 0;

        var cannotGoDirectionCount = 0;
        foreach (var direction in possibleDirections)
        {
            var nextLocation = startLocation.GetNextLocation(direction);
            var nextDistance = nextLocation.GetMaxSqmDistance(target);

            var canGoToDirection = Map.CanGoToDirection(creature, direction, tileEnterRule);
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

            if (nextDistance > currentDistance) weight++;
            if (nextLocation.GetSumSqmDistance(target) > startLocation.GetSumSqmDistance(target)) weight++;

            if (weight > directionWeight.Item2)
            {
                directionWeight.Item1 = direction;
                directionWeight.Item2 = weight;
            }
        }

        if (directionWeight.Item1 != Direction.None)
        {
            directions = new[] { directionWeight.Item1 };
            return true;
        }

        if (canGoIndex > 0)
        {
            var randonIndex = GameRandom.Random.Next(0, maxValue: canGoIndex);
            directions = new[] { canGoToDirections[randonIndex] };
            return true;
        }

        return true;
    }
}