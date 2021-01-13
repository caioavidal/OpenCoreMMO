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

            var tileArea = this;

            var tileNodes = node.Children.Select(child =>
             {
                 if (child.Type == NodeType.HouseTile || child.Type == NodeType.NormalTile)
                 {
                     return new TileNode(tileArea, child);
                 }
                 throw new Exception($"unknown tile nodes found.");
             }).ToList();

            Tiles.AddRange(tileNodes);

            var unknownNodes = node.Children.Count - Tiles.Count;
        }

        public ushort X { get; set; }
        public ushort Y { get; set; }
        public sbyte Z { get; set; }

        public List<TileNode> Tiles { get; set; }
    }
}
