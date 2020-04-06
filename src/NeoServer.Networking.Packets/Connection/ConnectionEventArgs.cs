using System;
using System.Collections.Generic;
using System.Text;
using NeoServer.Server.Contracts.Network;

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
