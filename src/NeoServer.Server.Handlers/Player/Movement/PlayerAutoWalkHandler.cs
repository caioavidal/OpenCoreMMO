using NeoServer.Game.Contracts;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Server.Handlers.Players
{
    public class PlayerAutoWalkHandler : PacketHandler
    {
        private readonly Game game;
        private readonly IMap map;



        public PlayerAutoWalkHandler(Game game,  IMap map)
        {
            this.game = game;
            this.map = map;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            //var autoWalk = new AutoWalkPacket(message);

            //var player = game.CreatureInstances[connection.PlayerId] as IThing;

            //foreach (var step in autoWalk.Steps)
            //{
            //    var nextTile = map.GetNextTile(player.Location, step); //todo temporary. the best place is not here
            //    dispatcher.Dispatch(new MapToMapMovementCommand(player, player.Location, nextTile.Location));
            //}
        }
    }
}
