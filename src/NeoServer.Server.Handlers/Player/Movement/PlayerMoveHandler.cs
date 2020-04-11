using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums.Location;
using NeoServer.Server.Commands;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Contracts.Network.Enums;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Handlers.Players
{
    public class PlayerMoveHandler : PacketHandler
    {
        private readonly Game game;
        
        public PlayerMoveHandler(Game game)
        {
            this.game = game;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            Direction direction = ParseMovementPacket(message.IncomingPacket);

            if(game.CreatureManager.TryGetCreature(connection.PlayerId, out ICreature creature))
            {
                var nextTile = game.Map.GetNextTile(creature.Location, direction); //todo temporary. the best place is not here

                game.Dispatcher.AddEvent(new Event(new MapToMapMovementCommand(creature, creature.Location, nextTile.Location, game).Execute));
            }

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
