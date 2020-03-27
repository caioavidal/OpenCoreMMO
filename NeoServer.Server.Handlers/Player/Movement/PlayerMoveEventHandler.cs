using System;
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
    public class PlayerMoveEventHandler : PacketHandler
    {
        private readonly IAccountRepository _repository;
        private readonly ServerState _serverState;

        private readonly Game _game;
        private readonly IMap _map;
        private readonly MapScheduler _mapScheduler;

        public PlayerMoveEventHandler(IAccountRepository repository, ServerState serverState, Game game, IMap map, MapScheduler mapScheduler)
        {
            _repository = repository;
            _serverState = serverState;
            _game = game;
            _map = map;
            _mapScheduler = mapScheduler;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, Connection connection)
        {
            Direction direction = ParseMovementPacket(message.IncomingPacket);

            var player = _game.CreatureInstances[connection.PlayerId] as Player;

            _mapScheduler.Enqueue(() =>
            {
                PlayerMovementHandler.Handler(player, _map, direction, connection);
            });


        }

        private Direction ParseMovementPacket(GameIncomingPacketType walkPacket)
        {
            var direction = Direction.North;

            switch (walkPacket)
            {
                case GameIncomingPacketType.WalkEast:
                    direction = Direction.East;
                    break;
                case GameIncomingPacketType.WalkNorth:
                    direction = Direction.North;
                    break;
                case GameIncomingPacketType.WalkSouth:
                    direction = Direction.South;
                    break;
                case GameIncomingPacketType.WalkWest:
                    direction = Direction.West;
                    break;
                case GameIncomingPacketType.WalkNorteast:
                    direction = Direction.NorthEast;
                    break;
                case GameIncomingPacketType.WalkNorthwest:
                    direction = Direction.NorthWest;
                    break;
                case GameIncomingPacketType.WalkSoutheast:
                    direction = Direction.SouthEast;
                    break;
                case GameIncomingPacketType.WalkSouthwest:
                    direction = Direction.SouthWest;
                    break;
            }

            return direction;
        }
    }
}
