using System;
using NeoServer.Networking.Handlers;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Networking.Protocols;

public class LoginProtocol : Protocol
{
    private readonly Func<IConnection, IPacketHandler> _handlerFactory;

    public LoginProtocol(Func<IConnection, IPacketHandler> packetFactory)
    {
        _handlerFactory = packetFactory;
    }

    public override bool KeepConnectionOpen => false;

    public override void ProcessMessage(object sender, IConnectionEventArgs args)
    {
        var handler = _handlerFactory(args.Connection);
        handler.HandleMessage(args.Connection.InMessage, args.Connection);
    }

    public override string ToString() => "Login Protocol";
}