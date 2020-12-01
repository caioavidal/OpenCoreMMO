using BenchmarkDotNet.Disassemblers;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Common;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Events
{
    public class PlayerOperationFailedEventHandler
    {
        private readonly Game game;
        public PlayerOperationFailedEventHandler(Game game) => this.game = game;
        public void Execute(uint playerId, string message)
        {
            if (game.CreatureManager.GetPlayerConnection(playerId, out var connection))
            {
                connection.OutgoingPackets.Enqueue(new TextMessagePacket(message, TextMessageOutgoingType.MESSAGE_STATUS_DEFAULT));
                connection.Send();
            }
        }
    }
}
