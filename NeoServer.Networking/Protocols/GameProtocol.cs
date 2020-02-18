using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Protocols
{
    public class GameProtocol : IProtocol
    {
        public bool KeepConnectionOpen => throw new NotImplementedException();

        public void OnAcceptNewConnection(Connection connection, IAsyncResult ar)
        {
            throw new NotImplementedException();
        }

        public void PostProcessMessage(object sender, ConnectionEventArgs args)
        {
            throw new NotImplementedException();
        }

        public void ProcessMessage(object sender, ConnectionEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
