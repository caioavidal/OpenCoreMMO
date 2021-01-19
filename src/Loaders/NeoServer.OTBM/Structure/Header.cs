using NeoServer.OTB.Parsers;
using NeoServer.OTB.Structure;

namespace NeoServer.OTBM.Structure
{
    /// <summary>
    /// OTBM Header data
    /// </summary>
    public struct Header
    {
        /// <summary>
        /// OTBM version
        /// </summary>
        public uint Version { get; set; }

        /// <summary>
        /// Major version items accepted
        /// </summary>
        public byte MajorVersionItems { get; set; }

        /// <summary>
        /// Minor version items accepted
        /// </summary>
        public uint MinorVersionItems { get; set; }

        /// <summary>
        /// Map width
        /// </summary>
        public ushort Width { get; set; }

        /// <summary>
        /// Map height
        /// </summary>
        public ushort Heigth { get; set; }

        public Header(OTBNode node)
        {

            var stream = new OTBParsingStream(node.Data);

            Version = stream.ReadUInt32();
            Width = stream.ReadUInt16();
            Heigth = stream.ReadUInt16();
            MajorVersionItems = stream.ReadByte();
            stream.Skip(3);
            MinorVersionItems = stream.ReadUInt32();

            //todo: needs version validation 

        }

    }
}
