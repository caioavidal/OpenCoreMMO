using NeoServer.Networking.Packets;
using NeoServer.Networking.Packets.Outgoing;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Queue
{
    public class OutputStreamMessage
    {
        public OutputStreamMessage(Connection connection, OutgoingPacket packet)
        {
            Connection = connection;
            Packet = packet;
        }

        public void Send() => Connection.Send(Packet);

        public Connection Connection { get; }
        public OutgoingPacket Packet { get; }


    }
}
