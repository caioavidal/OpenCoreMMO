using System;
using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.World.Algorithms;

public class AStar
{
    private static readonly sbyte[,] AllNeighbors = {
        { -1, 0 }, { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, -1 }, { 1, -1 }, { 1, 1 }, { -1, 1 }
    };
    
    private AStarCondition _aStarCondition;
    
    public AStar()
    {
        _aStarCondition = new AStarCondition();
    }

    public bool GetPathMatching(IMap map, ICreature creature, Location targetPos, FindPathParams
        fpp, ITileEnterRule tileEnterRule, out Direction[] directions)
    {
        var pos = creature.Location;
        directions = Array.Empty<Direction>();
        var endPos = new Location();
        var dirList = new List<Direction>();
        var bestMatch = 0;
        var node = new Node(pos);
        var startPos = pos;
        var sX = Math.Abs(targetPos.X - pos.X);
        var sY = Math.Abs(targetPos.Y - pos.Y);
        AStarNode found = null;

        while (fpp.MaxSearchDist != 0 || node.ClosedNodes < 100)
        {
            var bestNode = node.GetBestNode();
            if (bestNode is null)
            {
                if (found is not null) break;
                return false;
            }

            var x = bestNode.X;
            var y = bestNode.Y;

            pos.X = (ushort)x;
            pos.Y = (ushort)y;

            if (_aStarCondition.Validate(map, startPos, pos, targetPos, ref bestMatch, fpp))
            {
                found = bestNode;
                endPos = pos;
                if (bestMatch == 0) break;
            }

            int dirCount;
            sbyte[,] neighbors;

            if (bestNode.Parent is not null)
            {
                var offsetX = bestNode.Parent.X - x;
                var offsetY = bestNode.Parent.Y - y;
                neighbors = GetNeighbors(offsetY, offsetX);
                dirCount = 5;
            }
            else
            {
                dirCount = 8;
                neighbors = AllNeighbors;
            }

            var f = bestNode.F;
            for (var i = 0; i < dirCount; ++i)
            {
                pos.X = (ushort)(x + neighbors[i, 0]);
                pos.Y = (ushort)(y + neighbors[i, 1]);

                ITile tile = null;
                var extraCost = 0;

                if (fpp.MaxSearchDist != 0 && (startPos.GetSqmDistanceX(pos) > fpp.MaxSearchDist ||
                                               startPos.GetSqmDistanceY(pos) > fpp.MaxSearchDist)) continue;

                if (fpp.KeepDistance && !map.IsInRange(startPos, pos, targetPos, fpp)) continue;

                var neighborNode = node.GetNodeByPosition(pos);
                tile = map[pos];

                if (neighborNode is not null)
                {
                    extraCost = neighborNode.ExtraCost;
                }
                else
                {
                    if (!tileEnterRule.ShouldIgnore(tile, creature)) continue;
                    if (tile is IDynamicTile walkableTile) extraCost = bestNode.GetTileWalkCost(creature, walkableTile);
                }

                var cost = bestNode.GetMapWalkCost(pos);
                var newF = f + cost + extraCost;

                if (neighborNode is not null)
                {
                    if (neighborNode.F <= newF) continue;

                    neighborNode.F = newF;
                    neighborNode.Parent = bestNode;
                    node.OpenNode(neighborNode);
                }
                else
                {
                    var dX = Math.Abs(targetPos.X - pos.X);
                    var dY = Math.Abs(targetPos.Y - pos.Y);
                    neighborNode = node.CreateOpenNode(bestNode, pos.X, pos.Y, newF,
                        ((dX - sX) << 3) + ((dY - sY) << 3) + (Math.Max(dX, dY) << 3), extraCost);

                    if (neighborNode is null)
                    {
                        if (found is not null) break;
                        return false;
                    }
                }
            }

            node.CloseNode(bestNode);
        }

        if (found is null) return false;

        var prevPos = new Location(endPos.X, endPos.Y, endPos.Z);
        found = found.Parent;

        while (found is not null)
        {
            pos.X = (ushort)found.X;
            pos.Y = (ushort)found.Y;

            var direction = GetDirection(prevPos, pos);
            dirList.Insert(0, direction);

            prevPos = pos;
            found = found.Parent;
        }

        directions = dirList.ToArray();
        return true;
    }

    private sbyte[,] GetNeighbors(int offsetY, int offsetX)
    {
        sbyte[,] neighbors;
        if (offsetY == 0)
            neighbors = offsetX == -1 ? NeighborsDirection.West : NeighborsDirection.East;
        else if (offsetX == 0)
            neighbors = offsetY == -1 ? NeighborsDirection.North : NeighborsDirection.South;
        else if (offsetY == -1)
            neighbors = offsetX == -1 ? NeighborsDirection.NorthWest : NeighborsDirection.NorthEast;
        else if (offsetX == -1)
            neighbors = NeighborsDirection.SouthWest;
        else
            neighbors = NeighborsDirection.SouthEast;
        return neighbors;
    }

    private Direction GetDirection(Location prevPos, Location pos)
    {
        var dx = pos.X - prevPos.X;
        var dy = pos.Y - prevPos.Y;

        switch (dx)
        {
            case 1 when dy == 1:
                return Direction.NorthWest;
            case 1 when dy == -1:
                return Direction.SouthWest;
            case 1:
                return Direction.West;
            case -1 when dy == 1:
                return Direction.NorthEast;
            case -1 when dy == -1:
                return Direction.SouthEast;
            case -1:
                return Direction.East;
            default:
            {
                switch (dy)
                {
                    case 1:
                        return Direction.North;
                    case -1:
                        return Direction.South;
                }
                break;
            }
        }
        
        return Direction.None;
    }
}