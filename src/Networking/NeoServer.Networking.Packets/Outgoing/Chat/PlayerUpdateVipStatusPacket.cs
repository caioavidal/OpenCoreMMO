using NeoServer.Game.Contracts.Chats;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class PlayerUpdateVipStatusPacket : OutgoingPacket
    {
        private readonly uint playerId;
        private readonly bool online;

        public PlayerUpdateVipStatusPacket(uint playerId, bool online)
        {
            this.playerId = playerId;
            this.online = online;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)(online ? GameOutgoingPacketType.OnlineStatusVip : GameOutgoingPacketType.OfflineStatusVip));
            message.AddUInt32(playerId);
            
        }
    }
}
