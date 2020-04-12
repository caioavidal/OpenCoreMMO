using System;

namespace NeoServer.Data.Model
{
    public class IpBanModel
    {
        public uint IP { get; set; }
        public string Reason { get; set; }
        public TimeSpan BannedAt { get; set; }
        public TimeSpan ExpiresAt { get; set; }
        public ushort BannedBy { get; set; }

    }
}
