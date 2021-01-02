using NeoServer.Game.Contracts.World;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface ICreatureFactory
    {
        IMonster CreateMonster(string name, ISpawnPoint spawn = null);
        IPlayer CreatePlayer(IPlayer playerModel);
    }
}
