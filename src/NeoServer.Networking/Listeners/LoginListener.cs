using NeoServer.Networking.Protocols;

namespace NeoServer.Networking.Listeners
{
    public class LoginListener : Listener
    {
        public LoginListener(LoginProtocol protocol) : base(7171, protocol) { }
    }
}
