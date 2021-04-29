using NeoServer.Game.Contracts.Creatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Common.Contracts.Creatures
{
    public interface IWalkToMechanism
    {
        void WalkTo(IPlayer player, Action action, Location.Structs.Location toLocation, bool secondChance = false);
    }
}
