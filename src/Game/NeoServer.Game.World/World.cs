using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.World.Map;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NeoServer.Game.World
{
    public class World
    {
        public int LoadedTilesCount { get; private set; }
        public int LoadedTownsCount => towns.Count();
        public int LoadedWaypointsCount => waypoints.Count();

        private readonly ConcurrentDictionary<Coordinate, ITown> towns = new ConcurrentDictionary<Coordinate, ITown>();
        private readonly ConcurrentDictionary<Coordinate, IWaypoint> waypoints = new ConcurrentDictionary<Coordinate, IWaypoint>();
        private readonly Region region = new Region();

        public ImmutableList<ISpawn> Spawns { get; private set; }

        public void AddTile(ITile newTile)
        {
            ushort x = newTile.Location.X;
            ushort y = newTile.Location.Y;

            var sector = region.CreateSector(newTile.Location.X, newTile.Location.Y, out var created);
         
            sector.AddTile(newTile);
            LoadedTilesCount++;
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
            if (sector is null) return false;

            tile = sector.GetTile(location);
            if (tile is null) return false;
            return true;
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
