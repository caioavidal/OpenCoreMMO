using NeoServer.Networking;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Messages;
using NeoServer.Server.Contracts.Repositories;
using NeoServer.Server.Model.Players;

namespace NeoServer.Server.Handlers.Authentication
{
    public class PlayerChangesModeEventHandler : PacketHandler
    {
        private readonly IAccountRepository _repository;
        private readonly ServerState _serverState;

        private readonly Game _game;
        private readonly World.Map.Map _map;

        public PlayerChangesModeEventHandler(IAccountRepository repository, ServerState serverState, Game game, World.Map.Map map)
        {
            _repository = repository;
            _serverState = serverState;
            _game = game;
            _map = map;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, Connection connection)
        {
            var changeMode = new ChangeModePacket(message);

            var player = _game.CreatureInstances[connection.PlayerId] as Player;

            player.SetFightMode(changeMode.FightMode);
            player.SetChaseMode(changeMode.ChaseMode);
            player.SetSecureMode(changeMode.SecureMode);
        }
    }
}
