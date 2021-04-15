using NeoServer.Game.Common;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts;

namespace NeoServer.Server.Events
{
    public class InvalidOperationEventHandler
    {
        private readonly IGameServer game;

        public InvalidOperationEventHandler(IGameServer game)
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