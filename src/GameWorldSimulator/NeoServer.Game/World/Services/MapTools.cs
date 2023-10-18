using System;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.World.Algorithms;

namespace NeoServer.Game.World.Services;

public class MapTool : IMapTool
{
    public MapTool(IMap map, IPathFinder pathFinder)
    {
        PathFinder = pathFinder;
        SightClearChecker = (from, to, checkFloor) =>
            SightClear.IsSightClear(map, from, to, checkFloor);
    }

    public IPathFinder PathFinder { get; }
    public Func<Location, Location, bool, bool> SightClearChecker { get; }
}