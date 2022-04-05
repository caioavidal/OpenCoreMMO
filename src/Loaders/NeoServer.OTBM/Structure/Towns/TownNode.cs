using NeoServer.Game.Common.Location.Structs;
using NeoServer.OTB.Parsers;
using NeoServer.OTB.Structure;

namespace NeoServer.OTBM.Structure.Towns;

public class TownNode
{
    public TownNode(OtbNode node)
    {
        var stream = new OtbParsingStream(node.Data);
        Id = stream.ReadUInt32();
        Name = stream.ReadString();

        Coordinate = stream.ReadCoordinate();
    }

    public uint Id { get; set; }
    public string Name { get; set; }
    public Coordinate Coordinate { get; set; }
}