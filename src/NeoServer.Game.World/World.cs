using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.World.Map;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NeoServer.Game.World
{
    public class World
    {
        //public bool HasLoaded(int x, int y, byte z) => tiles.Any();
        public int LoadedTilesCount { get; private set; }
        public int LoadedTownsCount => towns.Count();
        public int LoadedWaypointsCount => waypoints.Count();

        private readonly ConcurrentDictionary<Coordinate, ITown> towns = new ConcurrentDictionary<Coordinate, ITown>();
        private readonly ConcurrentDictionary<Coordinate, IWaypoint> waypoints = new ConcurrentDictionary<Coordinate, IWaypoint>();
        private readonly Region region = new Region();

        public ImmutableList<ISpawn> Spawns { get; private set; }

        public void AddTile(ITile newTile)
        {
            var x = newTile.Location.X;
            var y = newTile.Location.Y;
            var z = newTile.Location.Z;

            var sector = region.AddChild(x, y, 15);

            if (sector != null)
            {
                var northSector = region.GetSector(x, y - 8);
                if (northSector != null) northSector.South = sector;

                var westSector = region.GetSector(x - 8, y);
                if (westSector != null) westSector.East = sector;

                var southSector = region.GetSector(x, y + 8);
                if (southSector != null) sector.South = southSector;

                var eastSector = region.GetSector(x + 8, y);
                if (eastSector != null) sector.East = eastSector;
            }

            var floor = sector.AddFloor(z);
            int offSetX = x & 7;
            int offSetY = y & 7;

            var tile = floor.Tiles[offSetX, offSetY];
            if (tile == null)
            {
                floor.AddTile(newTile);
                LoadedTilesCount++;
            }
        }

        public void LoadSpawns(IEnumerable<ISpawn> spawns)
        {
            spawns.ThrowIfNull();

            Spawns = spawns.ToImmutableList();
        }

        public bool TryGetTile(ref Location location, out ITile tile)
        {
            tile = null;
            var sector = region.GetSector(location.X, location.Y);
            if (sector == null)
            {
                return false;
            }
            var floor = sector.GetFloor(location.Z);

            tile = floor.Tiles == null ? null : floor.Tiles[location.X & 7, location.Y & 7];
            return tile != null;
        }

        public Sector GetSector(ushort x, ushort y) => region.GetSector(x, y);

        public IEnumerable<ICreature> GetSpectators(ref SpectatorSearch search) => region.GetSpectators(ref search);

        public void AddTown(ITown town)
        {
            town.ThrowIfNull();
            towns[town.Coordinate] = town;
        }

        public bool TryGetTown(Location location, out ITown town) => towns.TryGetValue(new Coordinate(location.X, location.Y, (sbyte)location.Z), out town);

        public void AddWaypoint(IWaypoint waypoint)
        {
            waypoint.ThrowIfNull();

            waypoints[waypoint.Coordinate] = waypoint;
        }

        public bool TryGetWaypoint(Location location, IWaypoint waypoint) => waypoints.TryGetValue(new Coordinate(location.X, location.Y, (sbyte)location.Z), out waypoint);

    }
}
