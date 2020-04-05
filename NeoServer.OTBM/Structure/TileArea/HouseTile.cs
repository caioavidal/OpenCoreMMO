using System;
using NeoServer.OTB.Enums;
using NeoServer.OTB.Parsers;
using NeoServer.OTB.Structure;
using NeoServer.OTBM.Enums;

namespace NeoServer.OTBM.Structure
{
    public class HouseTile : TileNode
    {
        public HouseTile(TileArea tileArea, OTBNode node) : base(tileArea, node)
        {
        }

        public uint HouseId { get; set; }
        public override NodeType NodeType => NodeType.HouseTile;

        public override void LoadTile(OTBParsingStream stream)
        {
            HouseId = stream.ReadUInt32();
        }
    }
}
