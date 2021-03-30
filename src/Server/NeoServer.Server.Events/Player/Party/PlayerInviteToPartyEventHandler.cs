using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures.Party;
using NeoServer.Networking.Packets.Outgoing.Party;
using NeoServer.Server.Contracts;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Events.Player
{
    public class PlayerInviteToPartyEventHandler
    {

        private readonly IGameServer game;

        public PlayerInviteToPartyEventHandler(IGameServer game)
        {
            this.game = game;
        }

        public void Execute(IPlayer leader, IPlayer invited, IParty party)
        {
            if (Guard.AnyNull(leader, invited, party)) return;

            foreach (var spectator in game.Map.GetPlayersAtPositionZone(leader.Location))
            {
                if (game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out var connection))
                {
                    connection.OutgoingPackets.Enqueue(new PartyEmblemPacket(leader, PartyEmblem.Yellow));
                    connection.Send();
                }
            }
        }
    }
}
