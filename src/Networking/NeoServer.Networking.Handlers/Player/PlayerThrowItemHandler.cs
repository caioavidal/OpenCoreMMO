using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Commands.Player;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Player
{
    public class PlayerThrowItemHandler : PacketHandler
    {
        private readonly IGameServer game;
        private readonly PlayerThrowItemCommand playerThrowItemCommand;

        public PlayerThrowItemHandler(IGameServer game, PlayerThrowItemCommand playerThrowItemCommand)
        {
            this.game = game;
            this.playerThrowItemCommand = playerThrowItemCommand;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var itemThrowPacket = new ItemThrowPacket(message);
            if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

            game.Dispatcher.AddEvent(new Event(2000,
                () => playerThrowItemCommand.Execute(player,
                    itemThrowPacket))); //todo create a const for 2000 expiration time
        }
    }
}