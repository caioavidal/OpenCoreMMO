using NeoServer.Game.Contracts.Creatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Creatures
{
    public record CreaturePathAccess (PathFinder FindPathToDestination, CanGoToDirection CanGoToDirection) : IPathAccess;
}
