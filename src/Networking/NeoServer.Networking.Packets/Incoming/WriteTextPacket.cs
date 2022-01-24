using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Networking.Packets.Incoming;

public class WriteTextPacket : IncomingPacket
{
    public WriteTextPacket(IReadOnlyNetworkMessage message)
    {
        WindowTextId = message.GetUInt32();
        Text = message.GetString();
    }

    public uint WindowTextId { get; }
    public string Text { get; }
}