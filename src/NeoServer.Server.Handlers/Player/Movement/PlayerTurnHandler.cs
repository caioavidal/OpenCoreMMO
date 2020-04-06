using NeoServer.Game.Commands;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
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
    public class PlayerTurnHandler : PacketHandler
    {
        private readonly Game game;
        private readonly IMap map;
        private readonly IDispatcher dispatcher;



        public PlayerTurnHandler(Game game, IDispatcher dispatcher, IMap map)
        {
            this.game = game;
            this.dispatcher = dispatcher;
            this.map = map;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            Direction direction = ParseTurnPacket(message.IncomingPacket);

            var player = game.CreatureInstances[connection.PlayerId] as ICreature;

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
