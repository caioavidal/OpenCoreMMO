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

            var player = _game.GetCreature(connection.PlayerId) as Player;

            player.SetFightMode(changeMode.FightMode);
            player.SetChaseMode(changeMode.ChaseMode);
            player.SetSecureMode(changeMode.SecureMode);
        }
    }
}
