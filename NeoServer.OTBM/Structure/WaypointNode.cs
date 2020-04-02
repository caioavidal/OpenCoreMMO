using NeoServer.Game.Enums.Location.Structs;
using NeoServer.OTBM.Helpers;

namespace NeoServer.OTBM.Structure
{
    public class WaypointNode
    {
        public string Name { get; set; }
        public Coordinate Coordinate { get; set; }


        public WaypointNode(OTBMNode node)
        {
            var stream = new OTBParsingStream(node.Data);

            Name = stream.ReadString();
         
            Coordinate = stream.ReadCoordinate();
        }
    }
}