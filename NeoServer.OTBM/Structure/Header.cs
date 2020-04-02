using System;
using NeoServer.OTBM.Helpers;

namespace NeoServer.OTBM.Structure
{
    public class Header
    {
        public uint Version { get; set; }
        public uint MajorVersionItems { get; set; }
        public uint MinorVersionItems { get; set; }
        public ushort Width { get; set; }
        public ushort Heigth { get; set; }

        public Header(OTBMNode node)
        {

            var stream = new OTBParsingStream(node.Data);

            Version = stream.ReadUInt32();
            Width = stream.ReadUInt16();
            Heigth = stream.ReadUInt16();
            MajorVersionItems = stream.ReadUInt32();
            MinorVersionItems = stream.ReadUInt32();

        }

    }
}
