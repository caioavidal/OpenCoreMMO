using System;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Common.Contracts.Network.Enums;
using Serilog.Core;

namespace NeoServer.Networking.Handlers
{
    public class NotImplementedPacketHandler : PacketHandler
    {
        private readonly Logger logger;
        private readonly GameIncomingPacketType packet;

        public NotImplementedPacketHandler(GameIncomingPacketType packet, Logger logger)
        {
            this.packet = packet;
            this.logger = logger;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var enumText = Enum.GetName(typeof(GameIncomingPacketType), packet);

            enumText = string.IsNullOrWhiteSpace(enumText) ? packet.ToString("x") : enumText;
            logger.Error("Incoming Packet not handled: {packet}", enumText);
        }
    }
}