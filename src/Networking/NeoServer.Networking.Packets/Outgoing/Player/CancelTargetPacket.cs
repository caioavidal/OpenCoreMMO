using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class CancelTargetPacket : OutgoingPacket
    {

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.CancelTarget);
            message.AddUInt32(0x00);
        }
    }
}
