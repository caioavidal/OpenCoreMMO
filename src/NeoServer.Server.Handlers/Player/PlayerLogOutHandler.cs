using NeoServer.Game.Commands;
using NeoServer.Game.Contracts;
using NeoServer.Networking;
using NeoServer.Networking.Packets.Messages;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Contracts.Repositories;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Handlers.Authentication
{
    public class PlayerLogOutHandler : PacketHandler
    {
        private readonly Game game;
        private readonly IDispatcher dispatcher;

        public PlayerLogOutHandler(Game game, IDispatcher dispatcher)
        {
            this.game = game;
            this.dispatcher = dispatcher;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var player = game.CreatureInstances[connection.PlayerId] as IPlayer;

            if (player == null)
            {
                return;
            }

            dispatcher.Dispatch(new PlayerLogOutCommand(player));
        }
    }
}
