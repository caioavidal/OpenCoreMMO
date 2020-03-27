using System;
using System.Collections.Generic;
using System.Text;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Protocols
{
    public abstract class OpenTibiaProtocol : IProtocol
    {
        public virtual bool KeepConnectionOpen { get; protected set; }

        public virtual void OnAcceptNewConnection(IConnection connection, IAsyncResult ar)
        {
            connection.OnAccept(ar);
        }

        public void PostProcessMessage(object sender, IConnectionEventArgs args)
        {
            if (!KeepConnectionOpen)
            {
                args.Connection.Close();
            }
            else
            {

                args.Connection.BeginStreamRead();
            }
        }

        public abstract void ProcessMessage(object sender, IConnectionEventArgs connection);
    }
}
