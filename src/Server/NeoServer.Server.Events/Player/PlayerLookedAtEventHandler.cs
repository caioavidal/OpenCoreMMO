using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Events.Player
{
    public class PlayerLookedAtEventHandler
    {
        private readonly IGameServer game;

        public PlayerLookedAtEventHandler(IGameServer game)
        {
            this.game = game;
        }

        public void Execute(IPlayer player, IThing thing, bool isClose)
        {
            if (game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection) is false) return;

            var text = isClose ? thing.CloseInspectionText : thing.InspectionText;
            var message = $"You see {text}."; //todo 
            connection.OutgoingPackets.Enqueue(new TextMessagePacket(message, TextMessageOutgoingType.Description));
            connection.Send();
        }
    }
}