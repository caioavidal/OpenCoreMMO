using NeoServer.Application.Common.PacketHandler;
using NeoServer.Networking.Packets.Network;

namespace NeoServer.Networking.Protocols;

public class LoginProtocol : Protocol
{
    private readonly PacketHandlerFactory _packetHandlerFactory;

    public LoginProtocol(PacketHandlerFactory packetHandlerFactory)
    {
        _packetHandlerFactory = packetHandlerFactory;
    }

    public override bool KeepConnectionOpen => false;

    public override void ProcessMessage(object sender, IConnectionEventArgs args)
    {
        var handler = _packetHandlerFactory.Create(args.Connection);
        handler?.HandleMessage(args.Connection.InMessage, args.Connection);
    }

    public override string ToString()
    {
        return "Login Protocol";
    }
}