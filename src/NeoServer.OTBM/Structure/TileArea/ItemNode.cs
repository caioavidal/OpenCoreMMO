using NeoServer.OTB.Enums;
using NeoServer.OTB.Parsers;
using NeoServer.OTB.Structure;
using NeoServer.OTBM.Enums;
using System;
using System.Collections.Generic;

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

            if (node.Type != NodeType.Item)
            {
                throw new Exception($"{tile.Coordinate}: Unknown node type");
            }

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
                case (UInt16)OTBMWorldItemId.FireFieldPvpLarge:
                    parsedItemId = (UInt16)OTBMWorldItemId.FireFieldPersistentLarge;
                    break;

                case (UInt16)OTBMWorldItemId.FireFieldPvpMedium:
                    parsedItemId = (UInt16)OTBMWorldItemId.FireFieldPersistentMedium;
                    break;

                case (UInt16)OTBMWorldItemId.FireFieldPvpSmall:
                    parsedItemId = (UInt16)OTBMWorldItemId.FireFieldPersistentSmall;
                    break;

                case (UInt16)OTBMWorldItemId.EnergyFieldPvp:
                    parsedItemId = (UInt16)OTBMWorldItemId.EnergyFieldPersistent;
                    break;

                case (UInt16)OTBMWorldItemId.PoisonFieldPvp:
                    parsedItemId = (UInt16)OTBMWorldItemId.PoisonFieldPersistent;
                    break;

                case (UInt16)OTBMWorldItemId.MagicWall:
                    parsedItemId = (UInt16)OTBMWorldItemId.MagicWallPersistent;
                    break;

                case (UInt16)OTBMWorldItemId.WildGrowth:
                    parsedItemId = (UInt16)OTBMWorldItemId.WildGrowthPersistent;
                    break;

                default:
                    break;
            }
            return parsedItemId;
        }

        private void ParseAttributes(OTBParsingStream stream)
        {

            while (!stream.IsOver)
            {
                var attribute = stream.ReadByte();

                if (attribute == 0)
                {
                    break;
                }

                ItemNodeAttributes.Add(new ItemNodeAttributeValue((ItemNodeAttribute)attribute, stream));
            }
        }
    }

}
