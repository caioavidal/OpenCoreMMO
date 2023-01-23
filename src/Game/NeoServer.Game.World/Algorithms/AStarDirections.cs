using System.Collections.Generic;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.World.Algorithms;

internal static class AStarDirections
{
    private static readonly List<Direction> DirList = new();
    public static Direction[] GetAll(AStarNode node, Location startPos, Location endPos)
    {
        DirList.Clear();
        var prevPos = new Location(endPos.X, endPos.Y, endPos.Z);
        node = node.Parent;

        while (node is not null)
        {
            var pos = new Location((ushort)node.X, (ushort)node.Y, startPos.Z);

            var direction = Next(prevPos, pos);
            DirList.Insert(0, direction);

            prevPos = pos;
            node = node.Parent;
        }

        return DirList.ToArray();
    }
    
    private static Direction Next(Location prevPos, Location pos)
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