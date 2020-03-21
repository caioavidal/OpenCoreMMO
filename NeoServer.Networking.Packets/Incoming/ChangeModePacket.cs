using NeoServer.Networking.Packets.Messages;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Security;

namespace NeoServer.Networking.Packets.Incoming
{
    public class ChangeModePacket : IncomingPacket
    {
        public FightMode FightMode { get; }
        public ChaseMode ChaseMode { get; }
        public byte SecureMode { get; }

        public ChangeModePacket(IReadOnlyNetworkMessage message)
        {
            FightMode = (FightMode) message.GetByte();
            ChaseMode = (ChaseMode)message.GetByte();
            SecureMode = message.GetByte();
        }
    }
}
