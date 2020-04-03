using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.OTB.Enums;
using NeoServer.OTB.Parsers;
using NeoServer.OTB.Structure;

namespace NeoServer.OTBM.Structure
{
    public class TileArea
    {
        public TileArea(OTBNode node)
        {
            var stream = new OTBParsingStream(node.Data);

            X = stream.ReadUInt16();
            Y = stream.ReadUInt16();
            Z = (sbyte)stream.ReadByte();

            var houseTiles = node.Children.Where(c => c.Type == NodeType.HouseTile)
                                          .Select(c => new HouseTile()); //todo iomap 250

            var normalTiles = node.Children.Where(c => c.Type == NodeType.NormalTile)
                                          .Select(c => new NormalTile(this, c));

            Tiles.AddRange(houseTiles);
            Tiles.AddRange(normalTiles);

            var unknownNodes = node.Children.Count - Tiles.Count;

            if (unknownNodes > 0)
            {
                throw new Exception($"{unknownNodes} unknown tile nodes found.");
            }
        }

        public ushort X { get; set; }
        public ushort Y { get; set; }
        public sbyte Z { get; set; }

        public List<TileNode> Tiles { get; set; } = new List<TileNode>();

    }
}
