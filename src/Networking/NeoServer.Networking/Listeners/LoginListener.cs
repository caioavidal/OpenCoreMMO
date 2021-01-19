using NeoServer.Networking.Protocols;
using Serilog.Core;

namespace NeoServer.Networking.Listeners
{
    public class LoginListener : Listener
    {
        public LoginListener(LoginProtocol protocol, Logger logger) : base(7171, protocol, logger) { }
    }
}
