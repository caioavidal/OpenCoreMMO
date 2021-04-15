using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts;
using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Server.Events.Player
{
    public class PlayerManaChangedEventHandler
    {
        private readonly IGameServer game;

        public PlayerManaChangedEventHandler(IGameServer game)
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
