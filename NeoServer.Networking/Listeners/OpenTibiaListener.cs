using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NeoServer.Networking.Listeners
{
    public class OpenTibiaListener : TcpListener, IOpenTibiaListener
    {
        public OpenTibiaListener(int port) : base(IPAddress.Any, port)
        {
        }

        public void BeginListening()
        {
            Start();
        }

        public void EndListening()
        {
            Stop();
        }
    }
}
