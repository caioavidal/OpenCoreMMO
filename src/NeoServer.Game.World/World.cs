using NeoServer.Game.Contracts.World;
using NeoServer.Game.Enums.Location.Structs;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NeoServer.Game.World
{
    public class World
    {
        public bool HasLoaded(int x, int y, byte z) => tiles.Any();
        public int LoadedTilesCount => tiles.Count();
        public int LoadedTownsCount => towns.Count();
        public int LoadedWaypointsCount => waypoints.Count();

        private readonly ConcurrentDictionary<Coordinate, ITile> tiles = new ConcurrentDictionary<Coordinate, ITile>();
        private readonly ConcurrentDictionary<Coordinate, ITown> towns = new ConcurrentDictionary<Coordinate, ITown>();
        private readonly ConcurrentDictionary<Coordinate, IWaypoint> waypoints = new ConcurrentDictionary<Coordinate, IWaypoint>();
        public ImmutableList<ISpawn> Spawns { get; private set; }

        public void AddTile(ITile tile)
        {
            tile.ThrowIfNull();

            var tileCoordinates = new Coordinate(
                x: tile.Location.X,
                y: tile.Location.Y,
                z: tile.Location.Z);

            tiles[tileCoordinates] = tile;
        }

        public void LoadSpawns(IEnumerable<ISpawn> spawns)
        {
            spawns.ThrowIfNull();

            Spawns = spawns.ToImmutableList();
        }

        public bool TryGetTile(Location location, out ITile tile) => tiles.TryGetValue(new Coordinate(location.X, location.Y, location.Z), out tile);



        public void AddTown(ITown town)
        {
            town.ThrowIfNull();
            towns[town.Coordinate] = town;
        }

        public bool TryGetTown(Location location, out ITown town) => towns.TryGetValue(new Coordinate(location.X, location.Y, location.Z), out town);

        public void AddWaypoint(IWaypoint waypoint)
        {
            waypoint.ThrowIfNull();

            waypoints[waypoint.Coordinate] = waypoint;
        }

        public bool TryGetWaypoint(Location location, IWaypoint waypoint) => waypoints.TryGetValue(new Coordinate(location.X, location.Y, location.Z), out waypoint);



    }
}
