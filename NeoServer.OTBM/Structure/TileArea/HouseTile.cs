using System;
using NeoServer.OTBM.Enums;

namespace NeoServer.OTBM.Structure
{
    public class HouseTile : TileNode
    {
        public int HouseId { get; set; }
        public override NodeType NodeType => NodeType.HouseTile;
    }
}
