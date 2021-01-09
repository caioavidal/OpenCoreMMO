using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Model.World.Map;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NeoServer.Game.World.Map
{
    public class Region
    {
        public const byte SECTOR_SIZE = 16;

        private ConcurrentDictionary<uint, Sector> Sectors = new();

        /// <summary>
        /// Creates a new sector or return if it already exists
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Sector CreateSector(uint x, uint y, out bool created)
        {
            created = false;

            var index = (x / SECTOR_SIZE) | ((y / SECTOR_SIZE) << 16);
            if (Sectors.TryGetValue(index, out var sector))
            {
                return sector;
            }

            var north = GetSector(x, (ushort)(y - SECTOR_SIZE));
            var west = GetSector((ushort)(x - SECTOR_SIZE), y);
            var south = GetSector(x, (ushort)(y + SECTOR_SIZE));
            var east = GetSector((ushort)(x + SECTOR_SIZE), y);

            var newSector = new Sector(north, south, west, east);
            Sectors.TryAdd(index, newSector);

            created = true;
            return newSector;
        }


        public Sector GetSector(uint x, uint y)
        {
            var index = (x / SECTOR_SIZE) | ((y / SECTOR_SIZE) << 16);
            Sectors.TryGetValue(index, out var sector);
            return sector;
        }
        public IEnumerable<ICreature> GetSpectators(ref SpectatorSearch search)
        {
            if (search.CenterPosition.Z >= Sector.MAP_MAX_LAYERS) return null;

            var spectators = new List<ICreature>();

            var minRangeX = search.RangeX.Min;
            var maxRangeX = search.RangeX.Max;
            var minRangeY = search.RangeY.Min;
            var maxRangeY = search.RangeY.Max;

            var minRangeZ = search.RangeZ.Min;
            var maxRangeZ = search.RangeZ.Max;
            
            if (minRangeX == (int)MapViewPort.ViewPortX && maxRangeX == (int)MapViewPort.ViewPortX && minRangeY == (int)MapViewPort.ViewPortY && maxRangeY == (int)MapViewPort.ViewPortY && search.Multifloor)
            {
            }

            var centerPos = search.CenterPosition;

            int min_y = centerPos.Y - minRangeY;
            int min_x = centerPos.X - minRangeX;
            int max_y = centerPos.Y + maxRangeY;
            int max_x = centerPos.X + maxRangeX;

            uint width = (uint)(max_x - min_x);
            uint height = (uint)(max_y - min_y);
            uint depth = (uint)(maxRangeZ - minRangeZ);

            int minoffset = centerPos.Z - maxRangeZ;
            int x1 = Math.Min(0xFFFF, Math.Max(0, (min_x + minoffset)));
            int y1 = Math.Min(0xFFFF, Math.Max(0, (min_y + minoffset)));

            int maxoffset = centerPos.Z- minRangeZ;
            int x2 = Math.Min(0xFFFF, Math.Max(0, (max_x + maxoffset)));
            int y2 = Math.Min(0xFFFF, Math.Max(0, (max_y + maxoffset)));

            int startx1 = x1 - (x1 & Sector.SECTOR_MASK);
            int starty1 = y1 - (y1 & Sector.SECTOR_MASK);
            int endx2 = x2 - (x2 & Sector.SECTOR_MASK);
            int endy2 = y2 - (y2 & Sector.SECTOR_MASK);

            Sector startSector = GetSector((uint)startx1, (uint)starty1);
            Sector sectorS = startSector;
            Sector sectorE;
            for (int ny = starty1; ny <= endy2; ny += SECTOR_SIZE)
            {
                sectorE = sectorS;
                for (int nx = startx1; nx <= endx2; nx += SECTOR_SIZE)
                {
                    if (sectorE is not null)
                    {
                        IEnumerable<ICreature> spectatorsList = (search.OnlyPlayers ? sectorE.Players : sectorE.Creatures);
                        foreach (var spec in spectatorsList)
                        {
                            ICreature creature = spec;

                            var creaturePosition = creature.Location;
                            if ((uint)(creaturePosition.Z - minRangeZ) <= depth)
                            {
                                var offsetZ = centerPos.GetOffSetZ(creaturePosition);
                                if ((uint)((creaturePosition.X - offsetZ) - min_x) <= width && (uint)((creaturePosition.Y - offsetZ) - min_y) <= height)
                                {
                                    spectators.Add(creature);
                                }
                            }
                        }
                        sectorE = sectorE.East;
                    }
                    else
                    {
                        sectorE = GetSector((ushort)(nx + SECTOR_SIZE), (ushort)ny);
                    }
                }

                if (sectorS is not null)
                {
                    sectorS = sectorS.South;
                }
                else
                {
                    sectorS = GetSector((ushort)startx1, (ushort)(ny + SECTOR_SIZE));
                }
            }

            return spectators;
        }
    }
}

