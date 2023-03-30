using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Networking.Packets.Incoming.Trade;

public class LookInTradePacket : IncomingPacket
{
    public LookInTradePacket(IReadOnlyNetworkMessage message)
    {
        CounterOffer = message.GetByte() == 0x01;
        Index = message.GetByte();
    }

    public byte Index { get; }
    public bool CounterOffer { get; }
}