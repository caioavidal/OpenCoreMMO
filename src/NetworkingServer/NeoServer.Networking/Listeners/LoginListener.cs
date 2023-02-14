using NeoServer.Networking.Protocols;
using Serilog;

namespace NeoServer.Networking.Listeners;

public class LoginListener : Listener
{
    public LoginListener(LoginProtocol protocol, ILogger logger)
        : base(7171, protocol, logger)
    {
    }
}