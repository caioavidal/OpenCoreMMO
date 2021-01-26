using NeoServer.Server.Contracts.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Networking.Packets.Incoming.Chat
{
    public class AddVipPacket : IncomingPacket
    {
        public string Name { get; set; }
        public AddVipPacket(IReadOnlyNetworkMessage message)
        {
            Name = message.GetString();
        }
    }
}
