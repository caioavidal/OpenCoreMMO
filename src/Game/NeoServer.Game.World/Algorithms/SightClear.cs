using System;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.World.Algorithms;

public static class SightClear
{
    /// <summary>
    ///     Checks if the sight is clear from location to target
    /// </summary>
    public static bool IsSightClear(IMap map, Location fromPosition, Location toPosition, bool checkFloor)
    {
        if (checkFloor && fromPosition.Z != toPosition.Z) return false;

        // Cast two converging rays and see if either yields a result.
        return CheckSightLine(map, fromPosition, toPosition) || CheckSightLine(map, toPosition, fromPosition);
    }

    private static bool CheckSightLine(IMap map, Location fromPosition, Location toPosition)
    {
        if (fromPosition == toPosition || fromPosition.IsNextTo(toPosition))
            return true;

        if (toPosition.Type != LocationType.Ground)
            return false;

        var start = fromPosition;
        var destination = toPosition;
        if (fromPosition.Z > toPosition.Z)
        {
            start = toPosition;
            destination = fromPosition;
        }

        var dx = destination.X - start.X;
        var dy = destination.Y - start.Y;
        var mx = dx > 0 ? 1 : -1;
        var my = dy > 0 ? 1 : -1;

        while (start.X != destination.X || start.Y != destination.Y)
        {
            var moveHorizontal = Math.Abs(dx);
            var moveVertical = Math.Abs(dy);
            var moveCross = Math.Abs(dx - dy);

            if (start.Y != destination.Y &&
                (start.X == destination.X || moveHorizontal > moveVertical || moveHorizontal > moveCross))
            {
                start.Y += (ushort)my;
                dy -= my;
            }

            if (start.X != destination.X &&
                (start.Y == destination.Y || moveVertical > moveHorizontal || moveVertical > moveCross))
            {
                start.X += (ushort)mx;
                dx -= mx;
            }

            var tile = map[start.X, start.Y, start.Z];
            if (tile is { BlockMissile: true }) return false;
        }

        while (start.Z != destination.Z)
        {
            var tile = map[start.X, start.Y, start.Z];

            if (tile is { HasThings: true }) return false;

            start.Z++;
        }

        return true;
    }
}