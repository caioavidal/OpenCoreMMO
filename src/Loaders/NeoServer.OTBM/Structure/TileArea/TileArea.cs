using System;
using System.Collections.Generic;
using NeoServer.OTB.Enums;
using NeoServer.OTB.Parsers;
using NeoServer.OTB.Structure;

namespace NeoServer.OTBM.Structure.TileArea
{
    public struct TileArea
    {
        public TileArea(OTBNode node)
        {
            var stream = new OTBParsingStream(node.Data);

            X = stream.ReadUInt16();
            Y = stream.ReadUInt16();
            Z = (sbyte) stream.ReadByte();

            Tiles = new List<TileNode>();

            var tileArea = this;

            foreach (var child in node.Children)
            {
                if (child.Type is not NodeType.HouseTile && child.Type is not  NodeType.NormalTile) 
                    throw new Exception("unknown tile nodes found."); 
                var tileNode = new TileNode(tileArea, child);
                
                Tiles.Add(tileNode);
            }
            
            var unknownNodes = node.Children.Count - Tiles.Count;
        }

        public ushort X { get; set; }
        public ushort Y { get; set; }
        public sbyte Z { get; set; }

        public List<TileNode> Tiles { get; set; }
    }
}