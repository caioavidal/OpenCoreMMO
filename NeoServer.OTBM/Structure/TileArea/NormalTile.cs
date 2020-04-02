using System;
using System.Linq;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.OTBM.Enums;
using NeoServer.OTBM.Helpers;

namespace NeoServer.OTBM.Structure
{
    public class NormalTile : TileNode
    {
        public override NodeType NodeType => NodeType.NormalTile;


        public NormalTile(TileArea tileArea, OTBMNode node)
        {
            var stream = new OTBParsingStream(node.Data);

            ushort x = (ushort)(tileArea.X + stream.ReadByte());
            ushort y = (ushort)(tileArea.Y + stream.ReadByte());

            Coordinate = new Coordinate(x, y, tileArea.Z);

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
