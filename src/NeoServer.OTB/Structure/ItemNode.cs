using NeoServer.OTB.Enums;
using NeoServer.OTB.Parsers;
using System;
using System.Collections.Immutable;

namespace NeoServer.OTB.Structure
{
    public class ItemNode
    {
        /// <summary>
        /// Gets the item flags
        /// </summary>
        /// <value></value>
        public uint Flags { get; set; }

        private T GetValue<T>(OTBItemAttribute attribute)
        {
            if (Attributes.TryGetValue(attribute, out IConvertible value))
            {
                return (T)value;
            }

            return default(T);
        }

        /// <summary>
        /// Gets the item's ServerId
        /// In case of ServerId is between 30000 and 30100, subtract 30000
        /// </summary>
        public ushort ServerId
        {
            get
            {
                var value = GetValue<ushort>(OTBItemAttribute.ServerId);
                if (value > 30000 && value < 30100)
                {
                    value -= 30000;
                }
                return value;
            }
        }

        public ushort ClientId => GetValue<ushort>(OTBItemAttribute.ClientId);

        public ushort Speed => GetValue<ushort>(OTBItemAttribute.Speed);

        public ushort WareId => GetValue<ushort>(OTBItemAttribute.WareId);

        public byte LightLevel => GetValue<byte>(OTBItemAttribute.LightLevel);
        public byte LightColor => GetValue<byte>(OTBItemAttribute.LightColor);
        public byte AlwaysOnTop => GetValue<byte>(OTBItemAttribute.TopOrder);

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