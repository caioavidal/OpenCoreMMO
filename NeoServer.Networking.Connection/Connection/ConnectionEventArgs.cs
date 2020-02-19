using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking
{
    public class ConnectionEventArgs : EventArgs
    {
        public Connection Connection { get; }

        public ConnectionEventArgs(Connection connection)
        {
            Connection = connection;
        }
    }
}
