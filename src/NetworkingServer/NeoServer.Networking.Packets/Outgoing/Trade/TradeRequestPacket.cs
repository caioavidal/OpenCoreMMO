using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Items.Items.Containers.Container.Queries;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing.Trade;

public class TradeRequestPacket : IOutgoingPacket
{
    public TradeRequestPacket(string playerName, IItem item, bool acknowledged = false)
    {
        PlayerName = playerName;
        Item = item;
        Acknowledged = acknowledged;
    }

    private string PlayerName { get; }
    private IItem Item { get; }
    private bool Acknowledged { get; }

    public void WriteToMessage(INetworkMessage message)
    {
        message.AddByte(Acknowledged
            ? (byte)GameOutgoingPacketType.AcknowlegdeTradeRequest
            : (byte)GameOutgoingPacketType.TradeRequest);

        message.AddString(PlayerName);

        if (Item is IContainer container)
        {
            var items = GetAllContainerItemsQuery.Get(container);
            
            message.AddByte((byte)items.Count);
            
            foreach (var item in items)
            {
                message.AddItem(item);
            }

            return;
        }

        message.AddByte(0x01);
        message.AddItem(Item);
    }
}