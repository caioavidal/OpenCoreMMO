using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.World.Map.Tiles;
using NeoServer.Server.Model.Players.Contracts;
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
        public ITile[,,] Tiles { get; set; } = new ITile[MAP_MAX_LAYERS, Region.SECTOR_SIZE, Region.SECTOR_SIZE];

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
            if (west is not null) west.East= this;
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
    }

    public readonly ref struct SpectatorSearch
    {
        public MinMax RangeX { get; }
        public MinMax RangeY { get; }
        public MinMax RangeZ { get; }
        public MinMax Y { get; }
        public MinMax X { get; }
        public bool OnlyPlayers { get; }
        public Location CenterPosition { get; }

        public SpectatorSearch(ref Location center, int minRangeX, int maxRangeX, int minRangeY, int maxRangeY, int minRangeZ, int maxRangeZ, bool onlyPlayers)
        {
            int minY = center.Y + minRangeY;
            int minX = center.X + minRangeX;
            int maxY = center.Y + maxRangeY;
            int maxX = center.X + maxRangeX;

            int minoffset = center.Z - maxRangeZ;
            ushort x1 = (ushort)Math.Min(0xFFFF, Math.Max(0, (minX + minoffset)));
            ushort y1 = (ushort)Math.Min(0xFFFF, Math.Max(0, (minY + minoffset)));

            int maxoffset = center.Z - minRangeZ;
            ushort x2 = (ushort)Math.Min(0xFFFF, Math.Max(0, (maxX + maxoffset)));
            ushort y2 = (ushort)Math.Min(0xFFFF, Math.Max(0, (maxY + maxoffset)));

            RangeX = new MinMax(x1 - (x1 % 8), x2 - (x2 % 8));
            RangeY = new MinMax(y1 - (y1 % 8), y2 - (y2 % 8));
            RangeZ = new MinMax(minRangeZ, maxRangeZ);
            Y = new MinMax(minY, maxY);
            X = new MinMax(minX, maxX);
            CenterPosition = center;
            OnlyPlayers = onlyPlayers;
        }
    }

}
