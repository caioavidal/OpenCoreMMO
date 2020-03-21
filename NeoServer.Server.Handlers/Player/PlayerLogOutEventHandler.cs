using NeoServer.Networking;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Messages;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Repositories;
using NeoServer.Server.Model;
using NeoServer.Server.Model.Creatures;
using NeoServer.Server.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public async override void HandlerMessage(IReadOnlyNetworkMessage message, Connection connection)
        {
            var player = _game.GetCreature(connection.PlayerId) as Player;

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
