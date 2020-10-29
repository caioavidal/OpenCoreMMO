using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.World
{
    public interface IPathFinder
    {
        IMap Map { get; set; }
        bool Find(ICreature creature,  Location target, FindPathParams findPathParams, out Direction[] directions);
        bool Find(ICreature creature, Location target, out Direction[] directions);
    }
}
