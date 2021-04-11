using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Common.Contracts.Items
{
    public interface ILootContainer
    {
        bool CanBeOpenedBy(IPlayer player);
    }
}
