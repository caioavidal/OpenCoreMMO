using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Common.Contracts.World;

public interface IWaypoint
{
    string Name { get; set; }
    Coordinate Coordinate { get; set; }
}