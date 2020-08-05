using NeoServer.Game.Enums.Location.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.World
{
    public interface ISpawn
    {
        Location Location { get; set; }
        byte Radius { get; set; }

        public interface IMonster
        {
            string Name { get; set; }
            Location Location { get; set; }
            ushort SpawnTime { get; set; }
        }
        IMonster[] Monsters { get; set; }
    }
}
