using NeoServer.Server.Commands;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Handlers.Authentication
{
    public class PlayerLogOutHandler : PacketHandler
    {
        private readonly IGameServer game;
        private readonly PlayerLogOutCommand playerLogOutCommand;

        public PlayerLogOutHandler(IGameServer game, PlayerLogOutCommand playerLogOutCommand)
        {
            this.game = game;
            this.playerLogOutCommand = playerLogOutCommand;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            if (game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player))
            {
                game.Dispatcher.AddEvent(new Event(() => playerLogOutCommand.Execute(player)));
            }
        }
    }
}
