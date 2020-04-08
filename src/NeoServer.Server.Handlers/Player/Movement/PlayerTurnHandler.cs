using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums.Location;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Contracts.Network.Enums;

namespace NeoServer.Server.Handlers.Players
{
    public class PlayerTurnHandler : PacketHandler
    {
        private readonly Game game;
        private readonly IMap map;
        



        public PlayerTurnHandler(Game game, IMap map)
        {
            this.game = game;
            
            this.map = map;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            Direction direction = ParseTurnPacket(message.IncomingPacket);

            var player = game.CreatureManager.GetCreature(connection.PlayerId) as ICreature;

            player.TurnTo(direction);
        }

        private Direction ParseTurnPacket(GameIncomingPacketType turnPacket)
        {
            var direction = Direction.North;

            switch (turnPacket)
            {
                case GameIncomingPacketType.TurnNorth:
                    direction = Direction.North;
                    break;
                case GameIncomingPacketType.TurnEast:
                    direction = Direction.East;
                    break;
                case GameIncomingPacketType.TurnSouth:
                    direction = Direction.South;
                    break;
                case GameIncomingPacketType.TurnWest:
                    direction = Direction.West;
                    break;
            }

            return direction;
        }
    }
}
