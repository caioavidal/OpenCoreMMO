using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing.Player;

public class ConditionIconPacket : OutgoingPacket
{
    private readonly ushort icons;

    public ConditionIconPacket(ushort icons)
    {
        this.icons = icons;
    }

    public override void WriteToMessage(INetworkMessage message)
    {
        message.AddByte((byte)GameOutgoingPacketType.PlayerConditions);
        message.AddUInt16(icons);
    }
}