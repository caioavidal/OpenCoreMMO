using NeoServer.Networking;
using NeoServer.Networking.Packets.Messages;
using NeoServer.Server.Contracts.Repositories;
using NeoServer.Server.Model.Players;

namespace NeoServer.Server.Handlers.Authentication
{
    public class PlayerLogOutEventHandler : PacketHandler
    {
        private readonly IAccountRepository _repository;
        private readonly ServerState _serverState;

        private readonly Game _game;
        private readonly World.Map.Map _map;

        public PlayerLogOutEventHandler(IAccountRepository repository, ServerState serverState, Game game, World.Map.Map map)
        {
            _repository = repository;
            _serverState = serverState;
            _game = game;
            _map = map;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, Connection connection)
        {
            var player = _game.CreatureInstances[connection.PlayerId];

            if (player == null)
            {
                return;
            }

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
