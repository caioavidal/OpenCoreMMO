using NeoServer.Game.Contracts;
using NeoServer.Networking;
using NeoServer.Networking.Packets.Messages;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Contracts.Repositories;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Handlers.Authentication
{
    public class PlayerLogOutEventHandler : PacketHandler
    {
        private readonly IAccountRepository _repository;
        private readonly ServerState _serverState;

        private readonly Game _game;
        private readonly IMap _map;

        public PlayerLogOutEventHandler(IAccountRepository repository, ServerState serverState, Game game, IMap map)
        {
            _repository = repository;
            _serverState = serverState;
            _game = game;
            _map = map;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var player = _game.CreatureInstances[connection.PlayerId] as IPlayer;

            if (player == null)
            {
                return;
            }

            player.Logout();

            _game.LogOutPlayer(connection);

            //if (Game.Instance.AttemptLogout(player)) todo
            {
                connection.Close();
            }
            //else
            //{
            //    ResponsePackets.Add(new TextMessagePacket
            //    {
            //        Type = MessageType.StatusSmall,
            //        Message = "You may not logout (test message)"
            //    });
            //}
        }
    }
}
