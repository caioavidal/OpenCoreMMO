using NeoServer.Game.Contracts;
using NeoServer.Game.Enums.Location.Structs;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace NeoServer.Server.World
{


    /// <summary>
    /// This class is meant to be a in-memory substitute for <see cref="SectorMapLoader"/>.
    /// The current implementation of <see cref="World"/> is slow and memory hungry, but it's meant
    /// to be just test the world loading functionality.
    /// We will refactor and improve it later.
    /// </summary>
    public class World {

		public byte PercentageComplete => 100;
		public bool HasLoaded(int x, int y, byte z) => _worldTiles.Any();
		public int LoadedTilesCount() => _worldTiles.Count();

		private readonly ConcurrentDictionary<Coordinate, ITile> _worldTiles = new ConcurrentDictionary<Coordinate, ITile>();
		
		public void AddTile(ITile tile) {
			if (tile == null)
				throw new ArgumentNullException(nameof(tile));

			var tilesCoordinates = new Coordinate(
				x: tile.Location.X,
				y: tile.Location.Y,
				z: tile.Location.Z);

			_worldTiles[tilesCoordinates] = tile;
		}

		public ITile GetTile(Location location)
		{
			if(_worldTiles.TryGetValue(new Coordinate(location.X, location.Y, location.Z), out ITile tile))
				return tile;

			return null;
		} 

	}
}
