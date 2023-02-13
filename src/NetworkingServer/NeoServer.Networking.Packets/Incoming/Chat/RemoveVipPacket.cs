using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Networking.Packets.Incoming.Chat;

public class RemoveVipPacket : IncomingPacket
{
    public RemoveVipPacket(IReadOnlyNetworkMessage message)
    {
        PlayerId = message.GetUInt32();
    }

    public uint PlayerId { get; set; }
}