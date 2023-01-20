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

    private bool PathCondition(IMap map, Location startPos, Location testPos,
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
            var n = node.GetBestNode();
            if (n is null)
            {
                if (found is not null) break;
                return false;
            }

            var x = n.X;
            var y = n.Y;

            pos.X = (ushort)x;
            pos.Y = (ushort)y;

            if (PathCondition(map, startPos, pos, targetPos, ref bestMatch, fpp))
            {
                found = n;
                endPos = pos;
                if (bestMatch == 0) break;
            }

            int dirCount;
            sbyte[,] neighbors;

            if (n.Parent is not null)
            {
                var offsetX = n.Parent.X - x;
                var offsetY = n.Parent.Y - y;
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

                dirCount = 5;
            }
            else
            {
                dirCount = 8;
                neighbors = AllNeighbors;
            }

            var f = n.F;
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
                    if (tile is IDynamicTile walkableTile) extraCost = n.GetTileWalkCost(creature, walkableTile);
                }

                var cost = n.GetMapWalkCost(pos);
                var newF = f + cost + extraCost;

                if (neighborNode is not null)
                {
                    if (neighborNode.F <= newF) continue;

                    neighborNode.F = newF;
                    neighborNode.Parent = n;
                    node.OpenNode(neighborNode);
                }
                else
                {
                    var dX = Math.Abs(targetPos.X - pos.X);
                    var dY = Math.Abs(targetPos.Y - pos.Y);
                    neighborNode = node.CreateOpenNode(n, pos.X, pos.Y, newF,
                        ((dX - sX) << 3) + ((dY - sY) << 3) + (Math.Max(dX, dY) << 3), extraCost);

                    if (neighborNode is null)
                    {
                        if (found is not null) break;
                        return false;
                    }
                }
            }

            node.CloseNode(n);
        }

        if (found is null) return false;

        int prevx = endPos.X;
        int prevy = endPos.Y;

        found = found.Parent;

        while (found is not null)
        {
            pos.X = (ushort)found.X;
            pos.Y = (ushort)found.Y;

            var dx = pos.X - prevx;
            var dy = pos.Y - prevy;

            prevx = pos.X;
            prevy = pos.Y;

            if (dx == 1)
            {
                if (dy == 1)
                    dirList.Insert(0, Direction.NorthWest);
                else if (dy == -1)
                    dirList.Insert(0, Direction.SouthWest);
                else
                    dirList.Insert(0, Direction.West);
            }
            else if (dx == -1)
            {
                if (dy == 1)
                    dirList.Insert(0, Direction.NorthEast);
                else if (dy == -1)
                    dirList.Insert(0, Direction.SouthEast);
                else
                    dirList.Insert(0, Direction.East);
            }
            else if (dy == 1)
            {
                dirList.Insert(0, Direction.North);
            }
            else if (dy == -1)
            {
                dirList.Insert(0, Direction.South);
            }

            found = found.Parent;
        }

        directions = dirList.ToArray();
        return true;
    }
}