using NeoServer.Game.Contracts.World;
using NeoServer.Game.Enums.Location.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Creatures
{
    public delegate void Born(IMonster monster, Location location);
    public interface IMonster:ICreature
    {
        event Born OnWasBorn;

        void Reborn();

        IMonsterType Metadata { get; }

        public ISpawnPoint Spawn { get;  }
        public bool FromSpawn => Spawn != null;

    }
}
