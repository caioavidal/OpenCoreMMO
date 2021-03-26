using Moq;
using NeoServer.Game.Tests;
using NeoServer.Server.Model.Players.Contracts;
using Xunit;

namespace NeoServer.Game.Creatures.Tests.Players
{
    public class PlayerPartyTest
    {
        [Fact]
        public void InviteToParty_When_Player_Already_In_A_Party_Dont_Invite()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100);

            var invitedPlayer = new Mock<IPlayer>();
            invitedPlayer.Setup(x => x.IsInParty).Returns(true);

            sut.InviteToParty(invitedPlayer.Object);

            Assert.Null(sut.Party);
        }

        [Fact]
        public void InviteToParty_When_Player_Is_Not_Leader_Dont_Invite()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100);

            var leader = PlayerTestDataBuilder.BuildPlayer(hp: 100);

            var invitedPlayer = PlayerTestDataBuilder.BuildPlayer(hp: 100);

            var invited = false;

            leader.InviteToParty(sut);

            leader.OnInviteToParty += (by, playerInvited, party) =>
            {
                if (playerInvited == invitedPlayer) invited = true;
            };

            sut.InviteToParty(invitedPlayer);

            Assert.False(invited);
        }
        [Fact]
        public void InviteToParty_Should_Invite()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100);

            var invitedPlayer = PlayerTestDataBuilder.BuildPlayer(hp: 100);

            var invited = false;

            sut.OnInviteToParty += (by, playerInvited, party) =>
            {
                if (playerInvited == invitedPlayer) invited = true;
            };

            sut.InviteToParty(invitedPlayer);

            Assert.True(invited);
        }
    }
}
