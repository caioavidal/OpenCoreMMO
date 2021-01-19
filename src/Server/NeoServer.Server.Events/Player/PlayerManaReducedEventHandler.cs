using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Events.Player
{
    public class PlayerManaChangedEventHandler
    {
        private readonly Game game;

        public PlayerManaChangedEventHandler(Game game)
        {
            this.game = game;
        }
        public void Execute(IPlayer player)
        {
            if (!game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection))
            {
                return;
            }
            connection.OutgoingPackets.Enqueue(new PlayerStatusPacket(player));
            connection.Send();

        }
    }
}
