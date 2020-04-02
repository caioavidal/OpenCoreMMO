using NeoServer.Game.Contracts.World;
using NeoServer.Game.Enums.Location.Structs;

namespace NeoServer.Server.Map
{
   

    public class Waypoint : IWaypoint
    {
        public string Name { get; set; }
        public Coordinate Coordinate { get; set; }
    }
}