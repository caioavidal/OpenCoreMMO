using NeoServer.OTB.Enums;
using NeoServer.OTB.Parsers;
using NeoServer.OTB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.OTBM.Structure
{
    public struct TileArea
    {
        public TileArea(OTBNode node)
        {
            var stream = new OTBParsingStream(node.Data);

            X = stream.ReadUInt16();
            Y = stream.ReadUInt16();
            Z = (sbyte)stream.ReadByte();

            Tiles = new List<TileNode>();

            foreach (var child in node.Children)
            {
                if(child.Type == NodeType.HouseTile || child.Type == NodeType.NormalTile)
                {
                    Tiles.Add(new TileNode(this, child));
                }
             
            }

            var unknownNodes = node.Children.Count - Tiles.Count;

            if (unknownNodes > 0)
            {
                throw new Exception($"{unknownNodes} unknown tile nodes found.");
            }
        }

        public ushort X { get; set; }
        public ushort Y { get; set; }
        public sbyte Z { get; set; }

        public List<TileNode> Tiles { get; set; } 

    }
}
