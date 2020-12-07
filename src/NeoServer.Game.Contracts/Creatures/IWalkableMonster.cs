using NeoServer.Game.Common.Location.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface IWalkableMonster
    {
        void Escape(Location fromLocation);
        bool LookForNewEnemy();
    }
}
