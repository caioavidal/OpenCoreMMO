using NeoServer.Game.Common.Players;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Incoming
{
    public class ChangeModePacket : IncomingPacket
    {
        public FightMode FightMode { get; }
        public ChaseMode ChaseMode { get; }
        public byte SecureMode { get; }

        public ChangeModePacket(IReadOnlyNetworkMessage message)
        {
            FightMode = (FightMode)message.GetByte();
            ChaseMode = (ChaseMode)message.GetByte();
            SecureMode = message.GetByte();
        }
    }
}
