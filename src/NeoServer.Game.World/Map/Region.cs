using NeoServer.Game.Contracts.Creatures;
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
        public Sector CreateSector(uint x, uint y)
        {
            var index = (x / SECTOR_SIZE) | ((y / SECTOR_SIZE) << 16);
            if(Sectors.TryGetValue(index, out var sector))
            {
                return sector;
            }

            var north = GetSector(x, y - SECTOR_SIZE);
            var west = GetSector(x - SECTOR_SIZE, y);
            var south = GetSector(x, y + SECTOR_SIZE);
            var east = GetSector(x + SECTOR_SIZE, y);

            var newSector = new Sector(north, west, south, east);
            Sectors.TryAdd(index, newSector);

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
            var spectators = new List<ICreature>();

            var startSector = GetSector((uint)search.RangeX.Min, (uint)search.RangeY.Min);
            var south = startSector;
            const int FLOOR_SIZE = 8;

            for (int ny = search.RangeY.Min; ny <= search.RangeY.Max; ny += FLOOR_SIZE)
            {

                Sector east = south;
                for (int nx = search.RangeX.Min; nx <= search.RangeX.Max; nx += FLOOR_SIZE)
                {
                    if (east != null)
                    {
                        //if (east.SpectatorsCache.Any())
                        //{
                        //    spectators.AddRange(east.SpectatorsCache);
                        //}
                        //else
                        //{
                        IEnumerable<ICreature> creatures = (search.OnlyPlayers ? east.Players : east.Creatures);

                        foreach (ICreature creature in creatures)
                        {
                            var cpos = creature.Location;
                            if (search.RangeZ.Min > cpos.Z || search.RangeZ.Max < cpos.Z)
                            {
                                continue;
                            }

                            int offsetZ = search.CenterPosition.GetOffSetZ(cpos);
                            if ((search.Y.Min + offsetZ) > cpos.Y || (search.Y.Max + offsetZ) < cpos.Y || (search.X.Min + offsetZ) > cpos.X || (search.X.Max + offsetZ) < cpos.X)
                            {
                                continue;
                            }

                            east.SpectatorsCache.Add(creature);
                            spectators.Add(creature);
                        }
                        // }
                        east = east.East;
                    }
                    else
                    {
                        east = GetSector((uint)nx + FLOOR_SIZE, (uint)ny);
                    }
                }

                if (south != null)
                {
                    south = south.South;
                }
                else
                {
                    south = GetSector((uint)search.RangeX.Min, (uint)ny + FLOOR_SIZE);
                }
            }

            return spectators;
        }
    }
}

