using NeoServer.Game.Common.Players;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Events.Player
{
    public class ItemAddedToInventoryEventHandler
    {
        private readonly Game game;

        public ItemAddedToInventoryEventHandler(Game game)
        {
            this.game = game;
        }
        public void Execute(IPlayer player, Slot slot)
        {
            if (game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection))
            {
                connection.OutgoingPackets.Enqueue(new PlayerInventoryItemPacket(player.Inventory, slot));
                connection.Send();
            }
        }
    }
}
