using System;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking
{
    public class ConnectionEventArgs : EventArgs, IConnectionEventArgs
    {
        public ConnectionEventArgs(Connection connection)
        {
            Connection = connection;
        }

        public IConnection Connection { get; }
    }
}