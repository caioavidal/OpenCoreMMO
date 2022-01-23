using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing.Chat;

public class PlayerOpenChannelPacket : OutgoingPacket
{
    private readonly ushort channelId;
    private readonly string name;

    public PlayerOpenChannelPacket(ushort channelId, string name)
    {
        this.channelId = channelId;
        this.name = name;
    }

    public override void WriteToMessage(INetworkMessage message)
    {
        message.AddByte((byte)GameOutgoingPacketType.OpenChannel);
        message.AddUInt16(channelId);
        message.AddString(name);
    }
}