using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.OTB.Enums;
using NeoServer.OTB.Parsers;
using NeoServer.OTB.Structure;
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


        public abstract void LoadTile(OTBParsingStream stream); //template method

        public TileNode(TileArea tileArea, OTBNode node)
        {
            var stream = new OTBParsingStream(node.Data);

            ushort x = (ushort)(tileArea.X + stream.ReadByte());
            ushort y = (ushort)(tileArea.Y + stream.ReadByte());

            Coordinate = new Coordinate(x, y, tileArea.Z);

            LoadTile(stream);

            ParseAttributes(stream);

            Items.AddRange(node.Children.Select(c => new ItemNode(this, c)));


        }

        private void ParseAttributes(OTBParsingStream stream)
        {
            if (!stream.IsOver)
            {
                NodeAttribute = (NodeAttribute)stream.ReadByte();

                if (IsFlag)
                {
                    Flag = ParseTileFlags((OTBMTileFlags)stream.ReadUInt32());
                }
                else if (IsItem)
                {
                    Items.Add(new ItemNode(stream));
                }
                else
                {
                    //Console.WriteLine($"{Coordinate}: Unknown tile attribute");
                    throw new Exception($"{Coordinate}: Unknown tile attribute");
                }
            }
        }

        private TileFlags ParseTileFlags(OTBMTileFlags newFlags)
        {
            TileFlags oldFlags = TileFlags.None;

            if ((newFlags & OTBMTileFlags.ProtectionZone) != 0)
                oldFlags |= TileFlags.ProtectionZone;
            else if ((newFlags & OTBMTileFlags.NoPvpZone) != 0)
                oldFlags |= TileFlags.NoPvpZone;
            else if ((newFlags & OTBMTileFlags.PvpZone) != 0)
                oldFlags |= TileFlags.PvpZone;

            if ((newFlags & OTBMTileFlags.NoLogout) != 0)
                oldFlags |= TileFlags.NoLogout;

            return oldFlags;
        }
    }
}
