using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Networking.Packets.Incoming.Shop;

public class PlayerSalePacket : IncomingPacket
{
    public PlayerSalePacket(IReadOnlyNetworkMessage message)
    {
        ItemClientId = message.GetUInt16();
        Count = message.GetByte();
        Amount = message.GetByte();
        IgnoreEquipped = message.GetByte() != 0;
    }

    public ushort ItemClientId { get; }
    public byte Count { get; }
    public byte Amount { get; set; }
    public bool IgnoreEquipped { get; set; }
}