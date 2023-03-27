using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing.Trade;

public class TradeRequestPacket : IOutgoingPacket
{
    private string PlayerName { get; }
    private IItem Item { get; }
    private bool Acknowledged { get; }

    public TradeRequestPacket(string playerName, IItem item, bool acknowledged = false)
    {
        PlayerName = playerName;
        Item = item;
        Acknowledged = acknowledged;
    }

    public void WriteToMessage(INetworkMessage message)
    {
        message.AddByte(Acknowledged ? (byte)GameOutgoingPacketType.AcknowlegdeTradeRequest : (byte)GameOutgoingPacketType.TradeRequest);

        message.AddString(PlayerName);

        message.AddByte(0x01);
        message.AddItem(Item);
    }
}