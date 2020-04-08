using NeoServer.Networking;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Messages;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Contracts.Repositories;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Handlers.Authentication
{
    public class PlayerChangesModeHandler : PacketHandler
    {
        private readonly Game game;

        public PlayerChangesModeHandler(IAccountRepository repository, Game game)
        {
            this.game = game;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var changeMode = new ChangeModePacket(message);

            var player = game.CreatureManager.GetCreature(connection.PlayerId) as IPlayer;

            player.SetFightMode(changeMode.FightMode);
            player.SetChaseMode(changeMode.ChaseMode);
            player.SetSecureMode(changeMode.SecureMode);

        }
    }
}
