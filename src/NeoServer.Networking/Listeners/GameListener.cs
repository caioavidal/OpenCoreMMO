using NeoServer.Networking.Protocols;
using Serilog.Core;

namespace NeoServer.Networking.Listeners
{
    public class GameListener : Listener
    {
        public GameListener(GameProtocol protocol, Logger logger) : base(7172, protocol, logger)
        {
        }
    }
}
