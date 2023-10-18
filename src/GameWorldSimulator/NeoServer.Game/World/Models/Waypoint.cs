using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.World.Models;

public struct Waypoint : IWaypoint
{
    public string Name { get; set; }
    public Coordinate Coordinate { get; set; }
}