using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures.Party;
using NeoServer.Networking.Packets.Outgoing.Party;
using NeoServer.Server.Contracts;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Events.Player
{
    public class PlayerLeftPartyEventHandler
    {

        private readonly IGameServer game;

        public PlayerLeftPartyEventHandler(IGameServer game)
        {
            this.game = game;
        }

        public void Execute(IPlayer member, IParty party)
        {
            if (Guard.AnyNull(member)) return;

            foreach (var spectator in game.Map.GetPlayersAtPositionZone(member.Location))
            {
                if (game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out var connection))
                {
                    connection.OutgoingPackets.Enqueue(new PartyEmblemPacket(member, PartyEmblem.None));
                    connection.Send();
                }
            }
        }
    }
}
