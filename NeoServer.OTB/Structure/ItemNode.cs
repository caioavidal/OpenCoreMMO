using System;
using System.Collections.Immutable;
using NeoServer.OTB.Enums;
using NeoServer.OTB.Parsers;

namespace NeoServer.OTB.Structure
{
    public class ItemNode
    {
        /// <summary>
        /// Gets the item flags
        /// </summary>
        /// <value></value>
        public uint Flags { get; set; }

        private ushort serverId;

        /// <summary>
        /// Gets the item's ServerId
        /// In case of ServerId is between 30000 and 30100, subtract 30000
        /// </summary>
        public ushort ServerId
        {
            get
            {
                var value = (ushort)Attributes[OTBItemAttribute.ServerId];
                if (value > 30000 && value < 30100)
                {
                    value -= 30000;
                }
                return value;
            }
        }

        public ushort ClientId => (ushort)Attributes[OTBItemAttribute.ClientId];

        public ushort Speed => (ushort)Attributes[OTBItemAttribute.Speed];

        public ushort WareId => (ushort)Attributes[OTBItemAttribute.WareId];

        public byte LightLevel => (byte)Attributes[OTBItemAttribute.LightLevel];
        public byte LightColor => (byte)Attributes[OTBItemAttribute.LightColor];
        public byte AlwaysOnTop => (byte)Attributes[OTBItemAttribute.TopOrder];

        /// <summary>
        /// Gets the item's type
        /// Used to set ItemType group and type
        /// </summary>
        /// <value></value>
        public NodeType Type { get; }

        private readonly ImmutableDictionary<OTBItemAttribute, IConvertible> Attributes;

        /// <summary>
        /// Creates ItemNode instance
        /// </summary>
        /// <param name="node"></param>
        public ItemNode(OTBNode node)
        {
            Type = node.Type;
            var stream = new OTBParsingStream(node.Data);

            Flags = stream.ReadUInt32();

            Attributes = new OTBParsingItemAttribute(stream).Attributes;

        }
    }
}