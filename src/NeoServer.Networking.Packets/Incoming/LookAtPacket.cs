using NeoServer.Game.Common.Location.Structs;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Incoming
{
    public class LookAtPacket : IncomingPacket
    {
        public Location Location { get; set; }
        public byte StackPosition { get; set; }

        public LookAtPacket(IReadOnlyNetworkMessage message)
        {
            Location = message.GetLocation();
            message.SkipBytes(2); //sprit id
            StackPosition = message.GetByte();
        }
    }
}
