using NeoServer.Game.Common.Location.Structs;
using NeoServer.OTB.Parsers;
using NeoServer.OTB.Structure;

namespace NeoServer.OTBM.Structure
{
    public struct WaypointNode
    {
        public string Name { get; set; }
        public Coordinate Coordinate { get; set; }

        public WaypointNode(OTBNode node)
        {
            var stream = new OTBParsingStream(node.Data);

            Name = stream.ReadString();

            Coordinate = stream.ReadCoordinate();
        }
    }
}