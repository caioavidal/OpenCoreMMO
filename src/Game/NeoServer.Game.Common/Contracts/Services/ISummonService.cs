using NeoServer.Game.Contracts.Creatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Common.Contracts.Services
{
    public interface ISummonService
    {
        IMonster Summon(IMonster master, string summonName);
    }
}
