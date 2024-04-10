using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Texts;
using NeoServer.Networking.Packets.Network;

namespace NeoServer.Networking.Packets.Outgoing.Effect;

public class AnimatedTextPacket(Location location, TextColor color, string text) : OutgoingPacket
{
    public override void WriteToMessage(INetworkMessage message)
    {
        message.AddByte((byte)GameOutgoingPacketType.AnimatedText);
        message.AddLocation(location);
        message.AddByte((byte)color);
        message.AddString(text);
    }
}