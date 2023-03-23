using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing.Trade;

public class TradeRequestPacket:IOutgoingPacket
{
    public string PlayerName { get; }
    public IItem Item { get; }

    public TradeRequestPacket( string playerName, IItem item)
    {
        PlayerName = playerName;
        Item = item;
    }
    public void WriteToMessage(INetworkMessage message)
    {
        message.AddByte((byte)GameOutgoingPacketType.TradeRequest);
        
        message.AddString(PlayerName);
        
        message.AddByte(0x01);
        message.AddItem(Item);
    }
}