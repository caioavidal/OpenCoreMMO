using NeoServer.Game.Commands;
using NeoServer.Game.Contracts;
using NeoServer.Game.Enums.Location;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Contracts.Network.Enums;
using NeoServer.Server.Contracts.Repositories;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Schedulers;

namespace NeoServer.Server.Handlers.Players
{
    public class PlayerMoveHandler : PacketHandler
    {
        private readonly Game game;
        private readonly IMap map;
        private readonly IDispatcher dispatcher;



        public PlayerMoveHandler(Game game, IDispatcher dispatcher, IMap map)
        {
            this.game = game;
            this.dispatcher = dispatcher;
            this.map = map;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            Direction direction = ParseMovementPacket(message.IncomingPacket);

            var player = game.CreatureInstances[connection.PlayerId] as IThing;

            var nextTile = map.GetNextTile(player.Location, direction); //todo temporary. the best place is not here

            dispatcher.Dispatch(new MapToMapMovementCommand(player, player.Location, nextTile.Location));
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
