using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface IPathAccess
    {
        PathFinder FindPathToDestination { get; }
        CanGoToDirection CanGoToDirection { get; }
    }
}
