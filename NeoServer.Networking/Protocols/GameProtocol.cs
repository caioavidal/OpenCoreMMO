using NeoServer.Networking.Packets;
using NeoServer.Networking.Packets.Outgoing;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Protocols
{
    public class GameProtocol : OpenTibiaProtocol
    {
        public override bool KeepConnectionOpen => true;

        public override void OnAcceptNewConnection(Connection connection, IAsyncResult ar)
        {
            //throw new NotImplementedException();
            Console.WriteLine("Game OnAcceptNewConnection");
            base.OnAcceptNewConnection(connection, ar);
            HandlerFirstConnection(connection);
        }

        public void HandlerFirstConnection(Connection connection)
        {
            connection.Send(new FirstConnectionPacket(), false);
        }

    
        public override void ProcessMessage(object sender, ConnectionEventArgs connection)
        {
        }
    }
}
