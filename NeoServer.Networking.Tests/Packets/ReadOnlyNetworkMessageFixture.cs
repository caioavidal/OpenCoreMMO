using NeoServer.Networking.Packets.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Tests.Packets
{
    public class ReadOnlyNetworkMessageFixture
    {
        public IReadOnlyNetworkMessage ReadOnlyNetworkMessage { get; private set; }
        public ReadOnlyNetworkMessageFixture()
        {
            var data = "test to get messages from readonly array";
            var buffer = Encoding.ASCII.GetBytes(data);

            ReadOnlyNetworkMessage = new ReadOnlyNetworkMessage(buffer);
        }
    }
}
