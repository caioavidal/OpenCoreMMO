using NeoServer.Networking.Protocols;
using Serilog;
using Serilog.Core;

namespace NeoServer.Networking.Listeners
{
    public class LoginListener : Listener
    {
        public LoginListener(LoginProtocol protocol, ILogger logger) : base(7171, protocol, logger)
        {
        }
    }
}