using System;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.World.Algorithms;
using NeoServer.Game.World.Map;

namespace NeoServer.Game.World.Services;

public class MapTool:IMapTool
{
    public MapTool(IMap map, IPathFinder pathFinder)
    {
        PathFinder = pathFinder;
        SightClearChecker = (from, to) => SightClear.IsSightClear(map, from, to, false);
    }

    public IPathFinder PathFinder { get; }
    public Func<Location, Location, bool> SightClearChecker { get; }
}