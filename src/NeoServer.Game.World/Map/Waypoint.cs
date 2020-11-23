using NeoServer.Game.Contracts.World;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.World.Map
{
    public struct Waypoint : IWaypoint
    {
        public string Name { get; set; }
        public Coordinate Coordinate { get; set; }
    }
}