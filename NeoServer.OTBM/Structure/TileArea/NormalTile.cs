using System;
using System.Linq;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.OTB.Enums;
using NeoServer.OTB.Parsers;
using NeoServer.OTB.Structure;
using NeoServer.OTBM.Enums;

namespace NeoServer.OTBM.Structure
{
    public class NormalTile : TileNode
    {
        public NormalTile(TileArea tileArea, OTBNode node) : base(tileArea, node)
        {
        }

        public override NodeType NodeType => NodeType.NormalTile;


        public override void LoadTile(OTBParsingStream stream) { }
    }
}
