using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.World;

namespace NeoServer.Game.World.Map
{
    public struct Town : ITown
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public Coordinate Coordinate { get; set; }
    }
}