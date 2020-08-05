using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Enums.Location.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.World.Spawns
{

    public class Spawn : ISpawn
    {
        public Location Location { get; set; }
        public byte Radius { get; set; }

        public class Monster : ISpawn.IMonster
        {
            public string Name { get; set; }
            public Location Location { get; set; }
            public ushort SpawnTime { get; set; }

        }
        public ISpawn.IMonster[] Monsters { get; set; }
    }
}
