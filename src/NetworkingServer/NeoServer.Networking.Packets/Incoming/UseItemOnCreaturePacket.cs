using NeoServer.Application.Common.Contracts.Network;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Networking.Packets.Incoming;

public class UseItemOnCreaturePacket : IncomingPacket
{
    public UseItemOnCreaturePacket(IReadOnlyNetworkMessage message)
    {
        FromLocation = message.GetLocation();
        ClientId = message.GetUInt16();
        FromStackPosition = message.GetByte();
        CreatureId = message.GetUInt32();
    }

    public Location FromLocation { get; }
    public ushort ClientId { get; }
    public byte FromStackPosition { get; set; }
    public uint CreatureId { get; }
}