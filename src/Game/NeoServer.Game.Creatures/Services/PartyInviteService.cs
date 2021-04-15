using NeoServer.Game.Chats;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Creatures.Model.Players;
using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Game.Creatures.Services
{
    public class PartyInviteService: IPartyInviteService
    {
        private readonly ChatChannelFactory chatChannelFactory;

        public PartyInviteService(ChatChannelFactory chatChannelFactory)
        {
            this.chatChannelFactory = chatChannelFactory;
        }

        public void Invite(IPlayer player, IPlayer invitedPlayer)
        {

            if (invitedPlayer is null || invitedPlayer.CreatureId == player.CreatureId) return;

            if (invitedPlayer.IsInParty)
            {
                OperationFailService.Display(player.CreatureId, $"{invitedPlayer.Name} is already in a party");
                return;
            }

            var partyCreatedNow = player.Party is null;
            IParty party = partyCreatedNow ? null : player.Party;

            if (partyCreatedNow)
            {
                var partyChannel = chatChannelFactory.CreatePartyChannel();
                party = new Party(player, partyChannel);
            }

            player.InviteToParty(invitedPlayer, party);
            invitedPlayer.ReceivePartyInvite(player, party);
        }
    }
}
