using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing.Player;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Events.Player
{
    public class PlayerWalkCancelledEventHandler
    {
        private readonly IGameServer game;

        public PlayerWalkCancelledEventHandler(IGameServer game)
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