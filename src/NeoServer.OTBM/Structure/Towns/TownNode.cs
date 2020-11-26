using NeoServer.Game.Common.Location.Structs;
using NeoServer.OTB.Parsers;
using NeoServer.OTB.Structure;

namespace NeoServer.OTBM.Structure
{
    public class TownNode
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public Coordinate Coordinate { get; set; }

        public TownNode(OTBNode node)
        {
            var stream = new OTBParsingStream(node.Data);
            Id = stream.ReadUInt32();
            Name = stream.ReadString();

            Coordinate = stream.ReadCoordinate();
        }
    }
}