using NeoServer.Game.Enums.Location.Structs;
using NeoServer.OTBM.Helpers;

namespace NeoServer.OTBM.Structure
{
    public class TownNode
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public Coordinate Coordinate { get; set; }

        public TownNode(OTBMNode node)
        {
            var stream = new OTBParsingStream(node.Data);
            Id = stream.ReadUInt32();
            Name = stream.ReadString();

            Coordinate = stream.ReadCoordinate();
        }
    }
}