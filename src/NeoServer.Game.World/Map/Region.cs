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
    }
}

