using System;
using NeoServer.Networking.Packets.Network;

namespace NeoServer.Networking.Packets.Connection;

public class ConnectionEventArgs : EventArgs, IConnectionEventArgs
{
    public ConnectionEventArgs(Connection connection)
    {
        Connection = connection;
    }

    public IConnection Connection { get; }
}