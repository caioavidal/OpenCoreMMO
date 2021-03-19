using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class LoginFailurePacket : OutgoingPacket
    {
        private readonly string text;
        public LoginFailurePacket(string text)
        {
            this.text = text;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte(0x0A);
            message.AddString(text);
        }
    }
}
