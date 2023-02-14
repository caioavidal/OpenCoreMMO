using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing.Chat;

public class PlayerAddVipPacket : OutgoingPacket
{
    private readonly uint playerId;
    private readonly string playerName;
    private readonly bool status;

    public PlayerAddVipPacket(uint playerId, string playerName, bool status)
    {
        this.playerId = playerId;
        this.playerName = playerName;
        this.status = status;
    }

    public override void WriteToMessage(INetworkMessage message)
    {
        message.AddByte((byte)GameOutgoingPacketType.AddVip);

        message.AddUInt32(playerId);
        message.AddString(playerName);
        message.AddByte(status ? (byte)1 : (byte)0);
    }
}