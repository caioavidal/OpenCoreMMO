using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Handlers;
using NeoServer.Server.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Networking.Handlers.Player
{
    public class PlayerInviteToPartyHandler : PacketHandler
    {
        private readonly IGameServer game;

        public PlayerInviteToPartyHandler(IGameServer game)
        {
            this.game = game;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var playerId = message.GetUInt32();
            if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;
            if (!game.CreatureManager.TryGetLoggedPlayer(playerId, out var invitedPlayer))
            {
                connection.Send(new TextMessagePacket("Invited player is not online.", TextMessageOutgoingType.Small));
                return;
            }


            game.Dispatcher.AddEvent(new Event(() => player.InviteToParty(invitedPlayer)));
        }
    }
}
