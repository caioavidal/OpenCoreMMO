using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Enums.Location.Structs;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace NeoServer.Server.World
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

        public void AddTile(ITile tile)
        {
            tile.ThrowIfNull();

            var tilesCoordinates = new Coordinate(
                x: tile.Location.X,
                y: tile.Location.Y,
                z: tile.Location.Z);

            tiles[tilesCoordinates] = tile;
        }

        public ITile GetTile(Location location)
        {
            if (tiles.TryGetValue(new Coordinate(location.X, location.Y, location.Z), out ITile tile))
                return tile;

            return null;
        }

        public void AddTown(ITown town)
        {
            town.ThrowIfNull();
            towns[town.Coordinate] = town;
        }

        public ITown GetTown(Location location)
        {
            if (towns.TryGetValue(new Coordinate(location.X, location.Y, location.Z), out ITown town))
                return town;

            return null;
        }

        public void AddWaypoint(IWaypoint waypoint)
        {
            waypoint.ThrowIfNull();

            waypoints[waypoint.Coordinate] = waypoint;
        }

        public IWaypoint GetWaypoint(Location location)
        {
            if (waypoints.TryGetValue(new Coordinate(location.X, location.Y, location.Z), out IWaypoint waypoint))
                return waypoint;

            return null;
        }

    }
}
