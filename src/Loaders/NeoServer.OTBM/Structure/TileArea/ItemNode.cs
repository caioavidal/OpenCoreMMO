using System;
using System.Collections.Generic;
using NeoServer.OTB.Enums;
using NeoServer.OTB.Parsers;
using NeoServer.OTB.Structure;
using NeoServer.OTBM.Enums;

namespace NeoServer.OTBM.Structure
{
    public struct ItemNode
    {
        public ushort ItemId { get; set; }
        public List<ItemNodeAttributeValue> ItemNodeAttributes { get; set; }

        public ItemNode(OTBParsingStream stream)
        {
            ItemNodeAttributes = default;
            ItemId = default;
            ItemId = ParseItemId(stream);
        }

        public ItemNode(TileNode tile, OTBNode node)
        {
            ItemNodeAttributes = new List<ItemNodeAttributeValue>();
            ItemId = default;

            if (node.Type != NodeType.Item) throw new Exception($"{tile.Coordinate}: Unknown node type");

            var stream = new OTBParsingStream(node.Data);

            ItemId = ParseItemId(stream);

            ParseAttributes(stream);
        }

        private ushort ParseItemId(OTBParsingStream stream)
        {
            var originalItemId = stream.ReadUInt16();
            var parsedItemId = originalItemId;

            switch (originalItemId)
            {
                case (ushort) OTBMWorldItemId.FireFieldPvpLarge:
                    parsedItemId = (ushort) OTBMWorldItemId.FireFieldPersistentLarge;
                    break;

                case (ushort) OTBMWorldItemId.FireFieldPvpMedium:
                    parsedItemId = (ushort) OTBMWorldItemId.FireFieldPersistentMedium;
                    break;

                case (ushort) OTBMWorldItemId.FireFieldPvpSmall:
                    parsedItemId = (ushort) OTBMWorldItemId.FireFieldPersistentSmall;
                    break;

                case (ushort) OTBMWorldItemId.EnergyFieldPvp:
                    parsedItemId = (ushort) OTBMWorldItemId.EnergyFieldPersistent;
                    break;

                case (ushort) OTBMWorldItemId.PoisonFieldPvp:
                    parsedItemId = (ushort) OTBMWorldItemId.PoisonFieldPersistent;
                    break;

                case (ushort) OTBMWorldItemId.MagicWall:
                    parsedItemId = (ushort) OTBMWorldItemId.MagicWallPersistent;
                    break;

                case (ushort) OTBMWorldItemId.WildGrowth:
                    parsedItemId = (ushort) OTBMWorldItemId.WildGrowthPersistent;
                    break;
            }

            return parsedItemId;
        }

        private void ParseAttributes(OTBParsingStream stream)
        {
            while (!stream.IsOver)
            {
                var attribute = stream.ReadByte();

                if (attribute == 0) break;

                ItemNodeAttributes.Add(new ItemNodeAttributeValue((ItemNodeAttribute) attribute, stream));
            }
        }
    }
}