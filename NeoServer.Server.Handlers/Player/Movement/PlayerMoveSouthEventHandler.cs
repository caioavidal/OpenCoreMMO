using System.Collections.Generic;
using NeoServer.Game.Contracts;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Networking;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Messages;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Repositories;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Schedulers.Map;

namespace NeoServer.Server.Handlers.Players
{
    public class PlayerMoveSouthEventHandler : PacketHandler
    {
        private readonly IAccountRepository _repository;

        private readonly Game _game;
        private readonly IMap _map;
        private readonly MapScheduler _mapScheduler;


        public PlayerMoveSouthEventHandler(IAccountRepository repository,
        Game game, IMap map, MapScheduler mapScheduler)
        {
            _repository = repository;
            _game = game;
            _map = map;
            _mapScheduler = mapScheduler;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, Connection connection)
        {
            var player = _game.CreatureInstances[connection.PlayerId] as Player;

            _mapScheduler.Enqueue(() =>
            {
                PlayerMovementHandler.Handler(player, _map, Direction.South, connection);
            });
        }
    }
}
