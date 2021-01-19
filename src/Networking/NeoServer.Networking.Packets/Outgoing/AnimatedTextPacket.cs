using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class AnimatedTextPacket : OutgoingPacket
    {
        private readonly TextColor color;
        private readonly string text;
        private readonly Location location;
        public AnimatedTextPacket(Location location, TextColor color, string message)
        {
            this.location = location;
            this.color = color;
            this.text = message;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.AnimatedText);
            message.AddLocation(location);
            message.AddByte((byte)color);
            message.AddString(text);
        }
    }
}
