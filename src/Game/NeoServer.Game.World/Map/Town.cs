using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.World.Map
{
    public struct Town : ITown
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public Coordinate Coordinate { get; set; }
    }
}