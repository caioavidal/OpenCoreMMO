using NeoServer.Game.Contracts;
using NeoServer.Game.Enums.Location.Structs;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace NeoServer.Server.World
{
    public class World
    {

        public byte PercentageComplete => 100;
        public bool HasLoaded(int x, int y, byte z) => worldTiles.Any();
        public int LoadedTilesCount() => worldTiles.Count();

        private readonly ConcurrentDictionary<Coordinate, ITile> worldTiles = new ConcurrentDictionary<Coordinate, ITile>();

        public void AddTile(ITile tile)
        {
            if (tile == null)
                throw new ArgumentNullException(nameof(tile));

            var tilesCoordinates = new Coordinate(
                x: tile.Location.X,
                y: tile.Location.Y,
                z: tile.Location.Z);

            worldTiles[tilesCoordinates] = tile;
        }

        public ITile GetTile(Location location)
        {
            if (worldTiles.TryGetValue(new Coordinate(location.X, location.Y, location.Z), out ITile tile))
                return tile;

            return null;
        }

    }
}
