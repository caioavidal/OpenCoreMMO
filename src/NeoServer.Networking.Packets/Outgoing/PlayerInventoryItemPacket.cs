using NeoServer.Game.Common.Players;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class PlayerInventoryItemPacket : OutgoingPacket
    {
        private readonly IInventory inventory;
        private readonly Slot slot;
        public PlayerInventoryItemPacket(IInventory inventory, Slot slot)
        {
            this.inventory = inventory;
            this.slot = slot;
        }

        public override void WriteToMessage(INetworkMessage message)
        {

            if (inventory[slot] == null)
            {
                message.AddByte((byte)GameOutgoingPacketType.InventoryEmpty);
                message.AddByte((byte)slot);
            }
            else
            {
                message.AddByte((byte)GameOutgoingPacketType.InventoryItem);
                message.AddByte((byte)slot);
                message.AddItem(inventory[slot]);
            }

        }
    }
}
