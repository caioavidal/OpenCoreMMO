using NeoServer.Game.Contracts.World;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class RemoveTileThingPacket : OutgoingPacket
    {
        private readonly byte stackPosition;
        private readonly ITile tile;

        public RemoveTileThingPacket(ITile tile, byte stackPosition)
        {
            this.tile = tile;
            this.stackPosition = stackPosition;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte) GameOutgoingPacketType.RemoveAtStackpos);
            message.AddLocation(tile.Location);
            message.AddByte(stackPosition);
        }
    }
}