using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures.Party;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Networking.Packets.Outgoing.Party;
using NeoServer.Server.Contracts;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Events.Player
{
    public class PlayerJoinedPartyEventHandler
    {

        private readonly IGameServer game;

        public PlayerJoinedPartyEventHandler(IGameServer game)
        {
            this.game = game;
        }

        public void Execute(IPlayer member, IParty party)
        {
            if (Guard.AnyNull(member, party)) return;

            foreach (var spectator in game.Map.GetPlayersAtPositionZone(member.Location))
            {
                if (!party.IsMember(spectator.CreatureId) && !party.IsLeader(spectator.CreatureId)) continue;

                if (!game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out var connection)) continue;

                if (spectator == member) //myself
                {
                    connection.OutgoingPackets.Enqueue(new PartyEmblemPacket(party.Leader, PartyEmblem.Yellow));
                    connection.OutgoingPackets.Enqueue(new TextMessagePacket($"You have joined {party.Leader.Name}'s party. Open the party channel to communicate with your companions.", TextMessageOutgoingType.Description));
                }
                else
                {
                    connection.OutgoingPackets.Enqueue(new TextMessagePacket($"{member.Name} has joined the party", TextMessageOutgoingType.Description));
                }

                if ((party.IsMember(spectator.CreatureId) || party.IsLeader(spectator.CreatureId)))
                {
                    //if spectator is member or leader, update new member emblem to blue
                    connection.OutgoingPackets.Enqueue(new PartyEmblemPacket(member, PartyEmblem.Blue));
                }

                connection.Send();

            }
        }
    }
}
