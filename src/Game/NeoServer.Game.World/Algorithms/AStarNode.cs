using System;
using System.Diagnostics.CodeAnalysis;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.World.Algorithms;

public class AStarNode : IEquatable<AStarNode>
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
    public int Heuristic { get; set; }
    public int ExtraCost { get; set; }

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