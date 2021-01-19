using NeoServer.Server.Contracts.Network;
using System;

namespace NeoServer.Networking
{

    public class ConnectionEventArgs : EventArgs, IConnectionEventArgs
    {
        public IConnection Connection { get; }

        public ConnectionEventArgs(Connection connection)
        {
            Connection = connection;
        }
    }
}
