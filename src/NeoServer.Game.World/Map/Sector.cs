using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.World.Map.Tiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.World.Map
{
    public class Sector : Region
    {
        public Sector South { get; set; }
        public Sector East { get; set; }
        public Floor[] Floors { get; set; } = new Floor[16];
        public List<ICreature> Creatures { get; set; } = new List<ICreature>();
        public Floor GetFloor(sbyte z) => Floors[z];
        public Floor AddFloor(sbyte z)
        {

            if (Floors[z].Tiles == null)
            {
                Floors[z].Tiles = new BaseTile[8, 8];
            }

            return Floors[z];
        }
    }
}
