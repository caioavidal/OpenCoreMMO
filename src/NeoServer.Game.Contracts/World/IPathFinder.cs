using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using System;
using System.Collections.Generic;
using System.Text;
using NeoServer.Game.Contracts.World.Tiles;

namespace NeoServer.Game.Contracts.World
{
    public interface IPathFinder
    {
        IMap Map { get; set; }
        
        bool Find(ICreature creature,  Location target, FindPathParams findPathParams, ITileEnterRule tileEnterRule, out Direction[] directions);
        bool Find(ICreature creature, Location target, out Direction[] directions);
    }
}
