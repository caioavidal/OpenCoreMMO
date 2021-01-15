using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Contracts.Network.Enums;
using Serilog.Core;

namespace NeoServer.Server.Handlers.Authentication
{
    public class NotImplementedPacketHandler : PacketHandler
    {
        private readonly GameIncomingPacketType packet;
        private readonly Logger logger;
        public NotImplementedPacketHandler(GameIncomingPacketType packet, Logger logger)
        {
            this.packet = packet;
            this.logger = logger;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            logger.Error("Incoming Packet not handled: {packet}", packet);
        }
    }
}
