using NeoServer.Game.Common.Location.Structs;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Incoming
{
    public class UseItemOnCreaturePacket : IncomingPacket
    {
        public Location FromLocation { get; }
        public ushort ClientId { get; }
        public byte FromStackPosition { get; set; }
        public uint CreatureId { get; }

        public UseItemOnCreaturePacket(IReadOnlyNetworkMessage message)
        {
            FromLocation = message.GetLocation();
            ClientId = message.GetUInt16();
            FromStackPosition = message.GetByte();
            CreatureId = message.GetUInt32();
        }
    }
}
