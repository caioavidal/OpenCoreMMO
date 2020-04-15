using NeoServer.Game.Contracts;
using NeoServer.Game.Enums.Location;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Events
{
    public class ThingMovedFailedEventHandler
    {
        private readonly Game game;

        public ThingMovedFailedEventHandler(Game game)
        {
            this.game = game;
        }
        public void Execute(IThing thing, PathError error)
        {
            if(thing is IPlayer)
            {

                if (game.CreatureManager.GetPlayerConnection(((IPlayer)thing).CreatureId, out var connection))
                {
                    connection.OutgoingPackets.Enqueue(new TextMessagePacket(TextMessageOutgoingParser.Parse(error), TextMessageOutgoingType.Small));
                    connection.Send();
                }
            }
        }
    }
}