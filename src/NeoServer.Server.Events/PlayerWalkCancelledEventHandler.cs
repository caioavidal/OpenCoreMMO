using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Events
{
    public class PlayerWalkCancelledEventHandler
    {
        private readonly Game game;

        public PlayerWalkCancelledEventHandler(Game game)
        {
            this.game = game;
        }
        public void Execute(IPlayer player)
        {
            if (game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection))
            {
                connection.OutgoingPackets.Enqueue(new PlayerWalkCancelPacket(player));
                connection.Send();
            }
        }
    }
}