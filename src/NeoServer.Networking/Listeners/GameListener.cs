using NeoServer.Networking.Protocols;

namespace NeoServer.Networking.Listeners
{
    public class GameListener : Listener
    {
        public GameListener(GameProtocol protocol) : base(7172, protocol)
        {
        }
    }
}
