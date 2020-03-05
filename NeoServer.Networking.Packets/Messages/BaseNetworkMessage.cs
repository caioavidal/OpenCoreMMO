namespace NeoServer.Networking.Packets
{
    using NeoServer.Server.Contracts.Network;
    using NeoServer.Server.Security;
    using System;
    using System.Linq;
    using System.Text;

    public class BaseNetworkMessage
    {
        protected byte[] Buffer { get; set; }
        public int Length { get; private set; } = 0;
    }
}
