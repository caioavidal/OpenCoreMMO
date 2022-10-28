using System;
using NeoServer.Networking.Handlers;
using NeoServer.Networking.Packets.Security;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Common.Contracts.Tasks;

namespace NeoServer.Networking.Protocols;

public class GameProtocol : Protocol
{
    private readonly Func<IConnection, IPacketHandler> _handlerFactory;
    private readonly IDispatcher _dispatcher;

    public GameProtocol(Func<IConnection, IPacketHandler> handlerFactory, IDispatcher dispatcher)
    {
        _handlerFactory = handlerFactory;
        _dispatcher = dispatcher;
    }

    public override bool KeepConnectionOpen => true;

    public override string ToString()
    {
        return "Game Protocol";
    }

    public override void OnAccept(IConnection connection)
    {
        HandlerFirstConnection(connection);
        base.OnAccept(connection);
    }

    public void HandlerFirstConnection(IConnection connection)
    {
        connection.SendFirstConnection();
    }

    public override void ProcessMessage(object sender, IConnectionEventArgs args)
    {
        var connection = args.Connection;

        HandleMessage(args, connection);
    }

    private void HandleMessage(IConnectionEventArgs args, IConnection connection)
    {
        if (connection.IsAuthenticated && !connection.Disconnected)
            Xtea.Decrypt(connection.InMessage, 6, connection.XteaKey);

        if (_handlerFactory(args.Connection) is not IPacketHandler handler) return;

        handler?.HandleMessage(args.Connection.InMessage, args.Connection);
    }
}