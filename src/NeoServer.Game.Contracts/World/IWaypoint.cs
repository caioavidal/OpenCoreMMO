using NeoServer.Game.Enums.Location.Structs;

namespace NeoServer.Game.Contracts.World
{
    public interface IWaypoint
    {
        string Name { get; set; }
        Coordinate Coordinate { get; set; }
    }
}