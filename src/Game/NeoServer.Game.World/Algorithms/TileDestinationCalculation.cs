using System;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.World.Algorithms;

public class TileDestinationCalculation
{
    public static ITile Calculate(ITile tile, IMap map)
    {
        return GetTileDestination(tile, map);
    }

    private static ITile GetTileDestination(ITile tile, IMap map)
    {
        var topItemOnStack = tile.TopItemOnStack;
        if (topItemOnStack.Metadata.Attributes.GetAttribute(ItemAttribute.Type)
            .Equals("teleport", StringComparison.InvariantCultureIgnoreCase))
        {
            if (!topItemOnStack.Metadata.Attributes.TryGetAttribute<Location>(ItemAttribute.TeleportDestination,
                    out var teleportDestination))
                return tile;

            return map[teleportDestination];
        }

        if (tile is not IDynamicTile toTile) return tile;

        bool HasFloorDestination(ITile tile, FloorChangeDirection direction)
        {
            return tile is IDynamicTile walkable && walkable.FloorDirection == direction;
        }

        var x = tile.Location.X;
        var y = tile.Location.Y;
        var z = tile.Location.Z;

        if (HasFloorDestination(tile, FloorChangeDirection.Down))
        {
            z++;

            var southDownTile = map[x, (ushort)(y - 1), z];

            if (HasFloorDestination(southDownTile, FloorChangeDirection.SouthAlternative))
            {
                y -= 2;
                return map[x, y, z] ?? tile;
            }

            var eastDownTile = map[(ushort)(x - 1), y, z];

            if (HasFloorDestination(eastDownTile, FloorChangeDirection.EastAlternative))
            {
                x -= 2;
                return map[x, y, z] ?? tile;
            }

            var downTile = map[x, y, z];

            if (downTile == null) return tile;

            if (HasFloorDestination(downTile, FloorChangeDirection.North)) ++y;
            if (HasFloorDestination(downTile, FloorChangeDirection.South)) --y;
            if (HasFloorDestination(downTile, FloorChangeDirection.SouthAlternative)) y -= 2;
            if (HasFloorDestination(downTile, FloorChangeDirection.East)) --x;
            if (HasFloorDestination(downTile, FloorChangeDirection.EastAlternative)) x -= 2;
            if (HasFloorDestination(downTile, FloorChangeDirection.West)) ++x;

            return map[x, y, z] ?? tile;
        }

        if (toTile.FloorDirection != default) //has any floor destination check
        {
            z--;

            if (HasFloorDestination(tile, FloorChangeDirection.North)) --y;
            if (HasFloorDestination(tile, FloorChangeDirection.South)) ++y;
            if (HasFloorDestination(tile, FloorChangeDirection.SouthAlternative)) y += 2;
            if (HasFloorDestination(tile, FloorChangeDirection.East)) ++x;
            if (HasFloorDestination(tile, FloorChangeDirection.EastAlternative)) x += 2;
            if (HasFloorDestination(tile, FloorChangeDirection.West)) --x;

            return map[x, y, z] ?? tile;
        }

        return tile;
    }
}