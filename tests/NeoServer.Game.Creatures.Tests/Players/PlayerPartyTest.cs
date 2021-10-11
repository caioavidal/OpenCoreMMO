using Moq;
using NeoServer.Game.Common.Contracts.Chats;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Creatures.Model.Players;
using NeoServer.Game.Tests.Helpers;
using Xunit;

namespace NeoServer.Game.Creatures.Tests.Players
{
    public class PlayerPartyTest
    {
        [Fact]
        public void InviteToParty_When_Player_Already_In_A_Party_Dont_Invite()
        {
            var sut = PlayerTestDataBuilder.Build(hp: 100);

            var invitedPlayer = new Mock<IPlayer>();
            invitedPlayer.Setup(x => x.IsInParty).Returns(true);

            var party = new Party(sut, new Mock<IChatChannel>().Object);

            sut.InviteToParty(invitedPlayer.Object, party);

            Assert.Null(sut.Party);
        }

        [Fact]
        public void InviteToParty_When_Player_Is_Not_Leader_Dont_Invite()
        {
            var sut = PlayerTestDataBuilder.Build(hp: 100);

            var leader = PlayerTestDataBuilder.Build(hp: 100);

            var invitedPlayer = PlayerTestDataBuilder.Build(hp: 100);

            var invited = false;

            var party = new Party(sut, new Mock<IChatChannel>().Object);

            leader.InviteToParty(sut, party);

            leader.OnInviteToParty += (by, playerInvited, party) =>
            {
                if (playerInvited == invitedPlayer) invited = true;
            };

            sut.InviteToParty(invitedPlayer, party);

            Assert.False(invited);
        }

        [Fact]
        public void InviteToParty_Should_Invite()
        {
            var sut = PlayerTestDataBuilder.Build(hp: 100);

            var invitedPlayer = PlayerTestDataBuilder.Build(hp: 100);

            var invited = false;

            sut.OnInviteToParty += (by, playerInvited, party) =>
            {
                if (playerInvited == invitedPlayer) invited = true;
            };
            var party = new Party(sut, new Mock<IChatChannel>().Object);

            sut.InviteToParty(invitedPlayer, party);

            Assert.True(invited);
        }
    }
}