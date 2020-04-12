using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class PlayerConditionsPacket : OutgoingPacket
    {
        public PlayerConditionsPacket()
        {

        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.PlayerConditions);
            message.AddUInt16(0x00);
        }
    }
}
