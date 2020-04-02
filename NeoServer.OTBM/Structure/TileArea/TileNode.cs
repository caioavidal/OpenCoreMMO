using System;
using System.Collections.Generic;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.OTBM.Enums;

namespace NeoServer.OTBM.Structure
{
    public abstract class TileNode : ITileNode
    {
        public Coordinate Coordinate { get; set; }
        public NodeAttribute NodeAttribute { get; set; }

        public bool IsFlag => NodeAttribute == NodeAttribute.TileFlags;
        public bool IsItem => NodeAttribute == NodeAttribute.Item;
        public abstract NodeType NodeType { get; }
        public TileFlags Flag { get; set; }
        public List<ItemNode> Items { get; set; } = new List<ItemNode>();
    }
}
