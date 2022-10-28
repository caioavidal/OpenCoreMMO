using System;
using NeoServer.Networking.Handlers;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Common.Contracts.Tasks;

namespace NeoServer.Networking.Protocols;

public class LoginProtocol : Protocol
{
    private readonly IDispatcher _dispatcher;
    private readonly Func<IConnection, IPacketHandler> _handlerFactory;

    public LoginProtocol(Func<IConnection, IPacketHandler> packetFactory, IDispatcher dispatcher)
    {
        _handlerFactory = packetFactory;
        _dispatcher = dispatcher;
    }

    public override bool KeepConnectionOpen => false;

    public override void ProcessMessage(object sender, IConnectionEventArgs args)
    {
        var handler = _handlerFactory(args.Connection);
        handler.HandleMessage(args.Connection.InMessage, args.Connection);
        // _dispatcher.AddEvent(new Event(() =>
        // {
        //   
        // }));
    }

    public override string ToString()
    {
        return "Login Protocol";
    }
}