using NeoServer.Game.Common;
using NeoServer.Game.Contracts.Items;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Events
{
    public class InvalidOperationEventHandler
    {
        private readonly Game game;

        public InvalidOperationEventHandler(Game game)
        {
            this.game = game;
        }
        public void Execute(IThing thing, InvalidOperation error)
        {
            if (thing is IPlayer)
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