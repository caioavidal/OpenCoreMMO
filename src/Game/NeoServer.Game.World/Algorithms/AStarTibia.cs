using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.World.Algorithms;

public class AStarTibia
{
    private bool PathCondition(IMap map, Location startPos, Location testPos,
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

    public bool GetPathMatching(IMap map, ICreature creature, Location targetPos, FindPathParams
        fpp, ITileEnterRule tileEnterRule, out Direction[] directions)
    {
        var pos = creature.Location;

        directions = Array.Empty<Direction>();

        var endPos = new Location();

        var dirList = new List<Direction>();

        var bestMatch = 0;

        var nodes = new Nodes(pos);

        var allNeighbors = new sbyte[,]
        {
            { -1, 0 }, { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, -1 }, { 1, -1 }, { 1, 1 }, { -1, 1 }
        };

        var startPos = pos;

        var sX = Math.Abs(targetPos.X - pos.X);
        var sY = Math.Abs(targetPos.Y - pos.Y);

        AStarNode found = null;

        while (fpp.MaxSearchDist != 0 || nodes.ClosedNodes < 100)
        {
            var n = nodes.GetBestNode();
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
                neighbors = allNeighbors;
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

                var neighborNode = nodes.GetNodeByPosition(pos);
                tile = map[pos];

                if (neighborNode is not null)
                {
                    extraCost = neighborNode.C;
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
                    nodes.OpenNode(neighborNode);
                }
                else
                {
                    var dX = Math.Abs(targetPos.X - pos.X);
                    var dY = Math.Abs(targetPos.Y - pos.Y);
                    neighborNode = nodes.CreateOpenNode(n, pos.X, pos.Y, newF,
                        ((dX - sX) << 3) + ((dY - sY) << 3) + (Math.Max(dX, dY) << 3), extraCost);

                    if (neighborNode is null)
                    {
                        if (found is not null) break;
                        return false;
                    }
                }
            }

            nodes.CloseNode(n);
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

internal class Nodes
{
    private readonly List<AStarNode> nodes = new(512);
    private readonly Dictionary<AStarNode, int> nodesIndexMap = new();
    private readonly Dictionary<AStarPosition, AStarNode> nodesMap = new();
    private readonly bool[] openNodes = new bool[512];
    private readonly AStarNode startNode;
    public int currentNode;

    public Nodes(Location location)
    {
        currentNode = 1;
        ClosedNodes = 0;
        openNodes[0] = true;

        startNode = new AStarNode(location.X, location.Y)
        {
            F = 0
        };

        nodes.Add(startNode);
        nodesIndexMap.Add(startNode, nodes.Count - 1);
        nodesMap.Add(new AStarPosition(location.X, location.Y), nodes[0]);
    }

    public int ClosedNodes { get; private set; }

    public AStarNode GetBestNode()
    {
        //   if (currentNode == 0) return null;

        var bestNodeF = int.MaxValue;
        var bestNode = -1;
        for (var i = 0; i < currentNode; ++i)
        {
            if (!openNodes[i]) continue;

            var diffNode = nodes[i].F + nodes[i].G;

            if (diffNode < bestNodeF)
            {
                bestNodeF = diffNode;
                bestNode = i;
            }
        }

        return bestNode != -1 ? nodes[bestNode] : null;
    }

    internal void CloseNode(AStarNode node)
    {
        var index = 0;
        var start = 0;
        while (true)
        {
            index = nodesIndexMap[node];
            if (openNodes[index] == false)
                start = ++index;
            else
                break;
        }

        if (index >= 512) return;

        openNodes[index] = false;
        ++ClosedNodes;
    }

    internal AStarNode CreateOpenNode(AStarNode parent, int x, int y, int newF, int heuristic, int extraCost)
    {
        if (currentNode >= 512) return null;

        var retNode = currentNode++;
        openNodes[retNode] = true;

        var node = new AStarNode(x, y)
        {
            F = newF,
            G = heuristic,
            C = extraCost,
            Parent = parent
        };
        nodes.Add(node);

        nodesIndexMap.Add(node, nodes.Count - 1);
        nodesMap.TryAdd(new AStarPosition(node.X, node.Y), node);
        return node;
    }

    internal AStarNode GetNodeByPosition(Location location)
    {
        nodesMap.TryGetValue(new AStarPosition(location.X, location.Y), out var foundNode);
        return foundNode;
    }

    internal void OpenNode(AStarNode node)
    {
        var index = nodesIndexMap[node];

        if (index >= 512) return;

        ClosedNodes -= openNodes[index] ? 0 : 1;
        openNodes[index] = true;
    }
}

internal readonly struct AStarPosition : IEquatable<AStarPosition>
{
    public AStarPosition(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; }
    public int Y { get; }

    public bool Equals([AllowNull] AStarPosition other)
    {
        return X == other.X && Y == other.Y;
    }

    public override bool Equals(object obj)
    {
        return obj is AStarPosition pos && Equals(pos);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}

internal class AStarNode : IEquatable<AStarNode>
{
    public AStarNode(int x, int y)
    {
        X = x;
        Y = y;
    }

    public AStarNode()
    {
    }

    public int F { get; set; }
    public int X { get; internal set; }
    public int Y { get; internal set; }
    public AStarNode Parent { get; set; }
    public int G { get; set; }
    public int C { get; set; }

    public bool Equals([AllowNull] AStarNode other)
    {
        return Equals(this, other);
    }

    public override bool Equals(object obj)
    {
        return obj is AStarNode node && Equals(this, node);
    }

    public override int GetHashCode()
    {
        return GetHashCode(this);
    }

    public bool Equals([AllowNull] AStarNode x, [AllowNull] AStarNode y)
    {
        return x.X == y.X && x.Y == y.Y;
    }

    public int GetHashCode([DisallowNull] AStarNode obj)
    {
        return HashCode.Combine(obj.X, obj.Y);
    }

    public int GetMapWalkCost(Location neighborPos)
    {
        // if (Math.Abs(X - neighborPos.X) == Math.Abs(Y - neighborPos.Y))
        //     //diagonal movement extra cost
        //     return 25;
        // return 10;

        return (Math.Abs(X - neighborPos.X) + Math.Abs(Y - neighborPos.Y) - 1) * 25 + 10;
    }

    public int GetTileWalkCost(ICreature creature, IDynamicTile tile)
    {
        var cost = 0;

        if (tile.GetTopVisibleCreature(creature) != null) cost += 10 * 3;

        if (tile.MagicField != null)
        {
            //if(creature.IsImmune() tile.MagicField.Type;
        }

        return cost;
    }
}

internal static class NeighborsDirection
{
    public static sbyte[,] West => new sbyte[,]
    {
        { 0, 1 },
        { 1, 0 },
        { 0, -1 },
        { 1, -1 },
        { 1, 1 }
    };

    public static sbyte[,] East => new sbyte[,]
    {
        { -1, 0 },
        { 0, 1 },
        { 0, -1 },
        { -1, -1 },
        { -1, 1 }
    };

    public static sbyte[,] North => new sbyte[,]
    {
        { -1, 0 },
        { 0, 1 },
        { 1, 0 },
        { 1, 1 },
        { -1, 1 }
    };

    public static sbyte[,] South => new sbyte[,]
    {
        { -1, 0 },
        { 1, 0 },
        { 0, -1 },
        { -1, -1 },
        { 1, -1 }
    };

    public static sbyte[,] NorthWest => new sbyte[,]
    {
        { 0, 1 },
        { 1, 0 },
        { 1, -1 },
        { 1, 1 },
        { -1, 1 }
    };

    public static sbyte[,] NorthEast => new sbyte[,]
    {
        { -1, 0 },
        { 0, 1 },
        { -1, -1 },
        { 1, 1 },
        { -1, 1 }
    };

    public static sbyte[,] SouthWest => new sbyte[,]
    {
        { 1, 0 },
        { 0, -1 },
        { -1, -1 },
        { 1, -1 },
        { 1, 1 }
    };

    public static sbyte[,] SouthEast => new sbyte[,]
    {
        { -1, 0 },
        { 0, -1 },
        { -1, -1 },
        { 1, -1 },
        { -1, 1 }
    };
}