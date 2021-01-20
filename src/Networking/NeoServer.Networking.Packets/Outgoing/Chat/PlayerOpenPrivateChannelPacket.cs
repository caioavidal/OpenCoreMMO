using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class PlayerOpenPrivateChannelPacket : OutgoingPacket
    {
        private readonly string receiver;

        public PlayerOpenPrivateChannelPacket(string receiver)
        {
            this.receiver = receiver;
        }

        //todo: this code is duplicated?
        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.OpenPrivateChannel);
            message.AddString(receiver);
        }
    }
}
