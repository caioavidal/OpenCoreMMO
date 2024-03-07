using NeoServer.Application.Common.Contracts.Network;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Networking.Packets.Incoming;

public class UseItemPacket : IncomingPacket
{
    public UseItemPacket(IReadOnlyNetworkMessage message)
    {
        Location = message.GetLocation();
        ClientId = message.GetUInt16();
        StackPosition = message.GetByte();
        Index = message.GetByte();
    }

    public Location Location { get; }
    public ushort ClientId { get; }
    public byte StackPosition { get; set; }
    public byte Index { get; set; }
}