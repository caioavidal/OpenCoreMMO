using NeoServer.Networking.Protocols;
using Serilog;

namespace NeoServer.Networking.Listeners;

public class GameListener : Listener
{
    public GameListener(GameProtocol protocol, ILogger logger) : base(8182,
        protocol, logger)
    {
    }
}