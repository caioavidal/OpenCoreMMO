using NeoServer.Game.Contracts.Creatures;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.World.Map
{
    public class Region
    {
        public Region[] Children { get; set; } = new Region[4];

        public Sector AddChild(int x, int y, int level)
        {
            if (!(this is Sector sector))
            {
                int index = ((x & 0x8000) >> 15) | ((y & 0x8000) >> 14);

                if (Children[index] == null)
                {
                    if (level != 3)
                    {
                        Children[index] = new Region();
                    }
                    else
                    {
                        Children[index] = new Sector();
                    }
                }
                return Children[index].AddChild(x * 2, y * 2, level - 1);

            }
            return sector;
        }
        public Sector GetSector(int x, int y)
        {
            if (this is Sector sector) return sector;

            var region = Children[((x & 0x8000) >> 15) | ((y & 0x8000) >> 14)];
            if (region == null)
            {
                return null;
            }
            return region.GetSector(x << 1, y << 1);
        }

        public List<ICreature> GetSpectators(ref SpectatorSearch search)
        {
            var spectators = new List<ICreature>();

            var startSector = GetSector(search.RangeX.Min, search.RangeY.Min);
            var south = startSector;
            const int FLOOR_SIZE = 8;

            for (int ny = search.RangeY.Min; ny <= search.RangeY.Max; ny += FLOOR_SIZE)
            {

                Sector east = south;
                for (int nx = search.RangeX.Min; nx <= search.RangeX.Max; nx += FLOOR_SIZE)
                {
                    if (east != null)
                    {
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

                            spectators.Add(creature);
                        }
                        east = east.East;
                    }
                    else
                    {
                        east = GetSector(nx + FLOOR_SIZE, ny);
                    }
                }

                if (south != null)
                {
                    south = south.South;
                }
                else
                {
                    south = GetSector(search.RangeX.Min, ny + FLOOR_SIZE);
                }
            }

            return spectators;
        }
    }
}

