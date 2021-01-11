using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Model.World.Map;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.World.Map
{
    public class Sector
    {
        public const byte MAP_MAX_LAYERS = 16;
        public const byte SECTOR_MASK = Region.SECTOR_SIZE - 1;
        public Sector South { get; private set; }
        public Sector East { get; private set; }
        public uint Floors { get; private set; }
        public HashSet<ICreature> Creatures { get; private set; } = new HashSet<ICreature>();
        public HashSet<ICreature> Players { get; private set; } = new HashSet<ICreature>();
        public List<ICreature> SpectatorsCache { get; private set; } = new List<ICreature>(32);
        private ITile[,,] Tiles = new ITile[MAP_MAX_LAYERS, Region.SECTOR_SIZE, Region.SECTOR_SIZE];

        public Sector(Sector north, Sector south, Sector west, Sector east)
        {
            UpdateNeighborsSetors(north, south, west, east);
        }

        public void AddTile(ITile tile)
        {
            var z = tile.Location.Z;
            var x = tile.Location.X;
            var y = tile.Location.Y;

            if (z >= MAP_MAX_LAYERS) return;

            CreateFloor(z);

            if (GetTile(tile.Location) is not null) return;

            Tiles[z, x & SECTOR_MASK, y & SECTOR_MASK] = tile;
        }

        public void CreateFloor(byte z) => Floors |= (uint)(1 << z);

        public void UpdateNeighborsSetors(Sector north, Sector south, Sector west, Sector east)
        {
            if (north is not null) north.South = this;
            if (west is not null) west.East = this;
            if (south is not null) South = south;
            if (east is not null) East = east;
        }

        public ITile GetTile(Location location)
        {
            if (location.Z >= MAP_MAX_LAYERS) return null;

            return Tiles[location.Z, location.X & SECTOR_MASK, location.Y & SECTOR_MASK];
        }


        public void AddCreature(ICreature creature)
        {
            Creatures.Add(creature);
            if (creature is IPlayer player)
            {
                Players.Add(player);
            }
            SpectatorsCache.Clear();
        }
        public void RemoveCreature(ICreature creature)
        {
            Creatures.Remove(creature);

            if (creature is IPlayer player)
                Players.Remove(player);

            SpectatorsCache.Clear();
        }

        //public HashSet<ICreature> GetSpectators(ref SpectatorSearch search)
        //{
        //    if (search.CenterPosition.Z >= MAP_MAX_LAYERS) return null;


        //}
    }

    public readonly ref struct SpectatorSearch
    {
        public MinMax RangeX { get; }
        public MinMax RangeY { get; }
        public MinMax RangeZ { get; }
        public bool OnlyPlayers { get; }
        public Location CenterPosition { get; }
        public bool Multifloor { get; }

        public SpectatorSearch(ref Location center, bool multifloor, int minRangeX, int maxRangeX, int minRangeY, int maxRangeY, bool onlyPlayers)
        {
            Multifloor = multifloor;
            int minY = minRangeY == 0 ? (int)MapViewPort.ViewPortY : minRangeY;
            int minX = minRangeX == 0 ? (int)MapViewPort.ViewPortX : minRangeX;
            int maxY = maxRangeY == 0 ? (int)MapViewPort.ViewPortY : maxRangeY;
            int maxX = maxRangeX == 0 ? (int)MapViewPort.ViewPortX : maxRangeX;

            RangeX = new MinMax(minX, maxX);
            RangeY = new MinMax(minY, maxY);
            RangeZ = GetRangeZ(center, multifloor);

            CenterPosition = center;
            OnlyPlayers = onlyPlayers;
        }

        private static MinMax GetRangeZ(Location center, bool multifloor)
        {
            var minRangeZ = center.Z;
            var maxRangeZ = center.Z;
            if (multifloor)
            {
                if (center.IsUnderground)
                {
                    minRangeZ = (byte)Math.Max(center.Z - 2, 0);
                    maxRangeZ = (byte)Math.Max(center.Z + 2, Sector.MAP_MAX_LAYERS - 1);
                }
                else if (center.Z == 6)
                {
                    minRangeZ = 0;
                    maxRangeZ = 8;
                }
                else if (center.Z == 7)
                {
                    minRangeZ = 0;
                    maxRangeZ = 9;
                }
                else
                {
                    minRangeZ = 0;
                    maxRangeZ = 7;
                }
            }

            return new MinMax(minRangeZ,maxRangeZ);
        }
    }

}
