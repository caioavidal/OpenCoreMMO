using NeoServer.Game.Chats;
using NeoServer.Game.Common.Contracts.Chats;
using NeoServer.Game.Creatures.Model.Players;
using NeoServer.Game.Creatures.Services;
using NeoServer.Game.Tests.Helpers;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NeoServer.Game.Creatures.Tests.Services
{
    public class PartyInviceServiceTest
    {
        [Fact]
        public void Invite_CreatesParty_WhenNeitherPlayerAreInAParty()
        {
            var partyLeader = PlayerTestDataBuilder.BuildPlayer(1);
            Assert.False(partyLeader.IsInParty);

            var invitedPlayer = PlayerTestDataBuilder.BuildPlayer(2);
            Assert.False(invitedPlayer.IsInParty);

            var chatChannelFactory = new ChatChannelFactory()
            {
                ChannelEventSubscribers = new List<IChatChannelEventSubscriber>()
            };
            var sharedExperienceService = new SharedExperienceService(new SharedExperienceConfiguration());
            var partyInviteService = new PartyInviteService(chatChannelFactory, sharedExperienceService);

            partyInviteService.Invite(partyLeader, invitedPlayer);

            Assert.True(partyLeader.IsInParty); // party leader has created a party by inviting someone.
            Assert.False(invitedPlayer.IsInParty);  // invited player has not yet accepted party invitation, therefore they are not in a party.
            Assert.True(partyLeader.Party.IsInvited(invitedPlayer));    // invited player should be listed as invited.
            Assert.Equal(1, partyLeader.Party.Members.Count);   // party leader should be added to the list of members upon creation.
            Assert.Equal(partyLeader, partyLeader.Party.Members.First()); // The party leader should be the only member at this point.
            Assert.True(partyLeader.Party.IsLeader(partyLeader));   // The party leader should be the party leader.
        }
    }
}