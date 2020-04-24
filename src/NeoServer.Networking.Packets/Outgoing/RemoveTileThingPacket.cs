using NeoServer.Game.Contracts.World;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class RemoveTileThingPacket : OutgoingPacket
    {
        private readonly ITile tile;
        private readonly byte stackPosition;
        public RemoveTileThingPacket(ITile tile, byte stackPosition)
        {
            this.tile = tile;
            this.stackPosition = stackPosition;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.RemoveAtStackpos);
            message.AddLocation(tile.Location);
            message.AddByte(stackPosition);
        }
    }
}
