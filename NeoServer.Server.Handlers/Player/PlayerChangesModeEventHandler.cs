using NeoServer.Networking;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Messages;
using NeoServer.Server.Contracts.Repositories;
using NeoServer.Server.Model.Players;

namespace NeoServer.Server.Handlers.Authentication
{
    public class PlayerChangesModeEventHandler : PacketHandler
    {
        private readonly Game _game;

        public PlayerChangesModeEventHandler(IAccountRepository repository, Game game)
        {
            _game = game;
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
