using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Loaders.Interfaces
{
    public interface IPlayerLoader
    {
        IPlayer Load(PlayerModel player);
        bool IsApplicable(PlayerModel player);
    }
}
