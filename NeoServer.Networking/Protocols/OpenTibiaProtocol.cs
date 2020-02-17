using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Protocols
{
    public abstract class OpenTibiaProtocol : IProtocol
    {
        public virtual bool KeepConnectionOpen { get; protected set; }

        public void OnAcceptNewConnection(Connection connection, IAsyncResult ar)
        {
            connection.OnAccept(ar);
        }

        public void PostProcessMessage(Connection connection)
        {
            throw new NotImplementedException();
        }

        public abstract void ProcessMessage(Connection connection);
    }
}
