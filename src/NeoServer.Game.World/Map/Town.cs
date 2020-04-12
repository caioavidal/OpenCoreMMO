using NeoServer.Game.Contracts.World;
using NeoServer.Game.Enums.Location.Structs;

namespace NeoServer.Game.World.Map
{
    public struct Town : ITown
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public Coordinate Coordinate { get; set; }
    }
}