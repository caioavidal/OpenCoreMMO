using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Server.Events.Player
{
    public class PlayerLookedAtEventHandler
    {
        private readonly IGameServer game;

        public PlayerLookedAtEventHandler( IGameServer game)
        {
            this.game = game;
        }
        public void Execute(IPlayer player, IThing thing, bool isClose)
        {
            if (game.CreatureManager.GetPlayerConnection(player.CreatureId, out IConnection connection) is false) return;

            var text = isClose ? thing.CloseInspectionText : thing.InspectionText;
            var message = $"You see {text}."; //todo 
            connection.OutgoingPackets.Enqueue(new TextMessagePacket(message, TextMessageOutgoingType.Description));
            connection.Send();

        }
    }
}
