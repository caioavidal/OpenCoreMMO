using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Items;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Server.Events.Player
{
    public class PlayerLookedAtEventHandler
    {
        private readonly Game game;

        public PlayerLookedAtEventHandler( Game game)
        {
            this.game = game;
        }
        public void Execute(IPlayer player, IThing thing, bool isClose)
        {
            if (game.CreatureManager.GetPlayerConnection(player.CreatureId, out IConnection connection) is false) return;

            var text = isClose ? thing.CloseInspectionText : thing.InspectionText;
            var message = $"You see {text}."; //todo 
            connection.OutgoingPackets.Enqueue(new TextMessagePacket(message, TextMessageOutgoingType.MessageInfoDescription));
            connection.Send();

        }
    }
}
