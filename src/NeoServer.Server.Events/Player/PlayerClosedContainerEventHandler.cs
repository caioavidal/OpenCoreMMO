using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.World;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Events
{
    public class PlayerClosedContainerEventHandler
    {
        private readonly IMap map;
        private readonly Game game;

        public PlayerClosedContainerEventHandler(IMap map, Game game)
        {
            this.map = map;
            this.game = game;
        }
        public void Execute(IPlayer player, byte containerId)
        {
            
            if (game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection))
            {
                connection.OutgoingPackets.Enqueue(new CloseContainerPacket(containerId));
                connection.Send();
            }
        }
    }
}
