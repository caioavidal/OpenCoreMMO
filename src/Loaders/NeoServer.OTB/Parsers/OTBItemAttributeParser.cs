using NeoServer.OTB.Enums;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace NeoServer.OTB.Parsers
{

    /// <summary>
    /// Responsible for parse the item attributes coming from OTB structure
    /// </summary>
    public sealed class OTBParsingItemAttribute
    {
        /// <summary>
        /// Dictionary containing the otb item attributes and its respective values
        /// </summary>
        /// <value></value>
        public ImmutableDictionary<OTBItemAttribute, IConvertible> Attributes { get; }

        private IDictionary<OTBItemAttribute, IConvertible> attributes =
        new Dictionary<OTBItemAttribute, IConvertible>();

        /// <summary>
        /// Creates OTBParsingItemAttribute instance
        /// </summary>
        /// <param name="stream"></param>
        public OTBParsingItemAttribute(OTBParsingStream stream)
        {
            stream.ThrowIfNull();
            while (!stream.IsOver)
            {
                Parse(stream);
            }
            Attributes = attributes.ToImmutableDictionary();
        }
        private void Parse(OTBParsingStream stream)
        {
            var attribute = (OTBItemAttribute)stream.ReadByte();
            var dataLength = stream.ReadUInt16();

            switch (attribute)
            {
                case OTBItemAttribute.ServerId:
                    dataLength.ThrowIfNotEqualsTo<ushort>(sizeof(ushort));

                    attributes.TryAdd(OTBItemAttribute.ServerId, stream.ReadUInt16());
                    break;

                case OTBItemAttribute.ClientId:
                    dataLength.ThrowIfNotEqualsTo<ushort>(sizeof(ushort));
                    attributes.TryAdd(OTBItemAttribute.ClientId, stream.ReadUInt16());
                    break;

                case OTBItemAttribute.Speed:
                    dataLength.ThrowIfNotEqualsTo<ushort>(sizeof(ushort));
                    attributes.TryAdd(OTBItemAttribute.Speed, stream.ReadUInt16());

                    break;
                case OTBItemAttribute.Light2:
                    //todo validation

                    attributes.TryAdd(OTBItemAttribute.LightLevel, (byte)stream.ReadUInt16());
                    attributes.TryAdd(OTBItemAttribute.LightColor, (byte)stream.ReadUInt16());
                    break;
                case OTBItemAttribute.TopOrder:
                    dataLength.ThrowIfNotEqualsTo<ushort>(sizeof(byte));
                    attributes.TryAdd(OTBItemAttribute.TopOrder, stream.ReadByte());
                    break;
                case OTBItemAttribute.WareId:
                    dataLength.ThrowIfNotEqualsTo<ushort>(sizeof(ushort));
                    attributes.TryAdd(OTBItemAttribute.WareId, stream.ReadUInt16());
                    break;

                default:
                    //skip unkown attributes
                    stream.Skip(dataLength);
                    break;
            }

        }
    }
}
