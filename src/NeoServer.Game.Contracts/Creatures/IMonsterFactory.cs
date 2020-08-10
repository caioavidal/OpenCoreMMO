using NeoServer.Game.Contracts.World;
using NeoServer.Game.Enums.Location.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface IMonsterFactory
    {
        IMonster Create(string name, ISpawnPoint spawn = null);
    }
}
