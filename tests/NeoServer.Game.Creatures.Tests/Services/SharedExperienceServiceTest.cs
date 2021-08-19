using Moq;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures.Model.Players;
using NeoServer.Game.Creatures.Services;
using NeoServer.Game.Creatures.Vocations;
using NeoServer.Game.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Xunit;

namespace NeoServer.Game.Creatures.Tests.Services
{
    public class SharedExperienceServiceTest
    {
        [Fact]
        public void PartyShareExperienceService_ThrowsNullArgumentException_WhenConfigurationIsNull()
        {
            var party = new Mock<IParty>().Object;
            Assert.Throws<ArgumentNullException>(() => new SharedExperienceService(null));
        }

        [Fact]
        public void IsExperienceSharingTurnedOn_ReturnsTrue_WhenConfiguredToAlwaysBeOn()
        {
            var playerOne = PlayerTestDataBuilder.BuildPlayer(id: 1);
            var playerTwo = PlayerTestDataBuilder.BuildPlayer(id: 2);

            var config = PartyTestDataBuilder.CreateSharedExperienceConfiguration(isSharedExperienceAlwaysOn: true);
            var inviteService = PartyTestDataBuilder.CreateInviteService(config);
            var party = PartyTestDataBuilder.CreateParty(inviteService, playerOne, playerTwo);

            party.SharedExperienceService.ExperienceSharingEnabled = false;

            var service = (SharedExperienceService)party.SharedExperienceService;
            Assert.True(service.IsExperienceSharingTurnedOn());
        }

        [Fact]
        public void IsExperienceSharingTurnedOn_ReturnsTrue_WhenPartySharedExperienceEnabled()
        {
            var playerOne = PlayerTestDataBuilder.BuildPlayer(id: 1);
            var playerTwo = PlayerTestDataBuilder.BuildPlayer(id: 2);

            var config = PartyTestDataBuilder.CreateSharedExperienceConfiguration(isSharedExperienceAlwaysOn: false);
            var inviteService = PartyTestDataBuilder.CreateInviteService(config);
            var party = PartyTestDataBuilder.CreateParty(inviteService, playerOne, playerTwo);

            party.SharedExperienceService.ExperienceSharingEnabled = true;

            var service = (SharedExperienceService)party.SharedExperienceService;
            Assert.True(service.IsExperienceSharingTurnedOn());
        }

        [Fact]
        public void IsExperienceSharingTurnedOn_ReturnsFalse_WhenPartySharedExperienceDisabled()
        {
            var playerOne = PlayerTestDataBuilder.BuildPlayer(id: 1);
            var playerTwo = PlayerTestDataBuilder.BuildPlayer(id: 2);

            var config = PartyTestDataBuilder.CreateSharedExperienceConfiguration(isSharedExperienceAlwaysOn: false);
            var inviteService = PartyTestDataBuilder.CreateInviteService(config);
            var party = PartyTestDataBuilder.CreateParty(inviteService, playerOne, playerTwo);

            party.SharedExperienceService.ExperienceSharingEnabled = false;

            var service = (SharedExperienceService)party.SharedExperienceService;
            Assert.False(service.IsExperienceSharingTurnedOn());
        }

        [InlineData(1, 0, true)]
        [InlineData(10, 5, true)]
        [InlineData(100, 20, true)]
        [InlineData(500, 100, true)]
        [InlineData(550, 550, true)]
        [InlineData(2500, 2000, true)]
        [InlineData(0, 1, false)]
        [InlineData(10, 20, false)]
        [InlineData(15, 20, false)]
        [Theory]
        public void DoesMonsterQuality_ReturnsTrue_WhenMonsterExperienceGreaterThanOrEqualToConfiguredMinimumExperience(uint monsterExperience, uint minimumExperience, bool expectedResult)
        {
            var playerOne = PlayerTestDataBuilder.BuildPlayer(id: 1);
            var playerTwo = PlayerTestDataBuilder.BuildPlayer(id: 2);

            var config = PartyTestDataBuilder.CreateSharedExperienceConfiguration(
                minimumMonsterExperienceToBeShared: minimumExperience
            );
            var inviteService = PartyTestDataBuilder.CreateInviteService(config);
            var party = PartyTestDataBuilder.CreateParty(inviteService, playerOne, playerTwo);

            var monsterMock = new Mock<IMonster>();
            monsterMock.Setup(x => x.Experience).Returns(monsterExperience);

            var service = (SharedExperienceService)party.SharedExperienceService;
            Assert.Equal(expectedResult, service.DoesMonsterQualify(monsterMock.Object));
        }

        [InlineData(false, (3d / 2d), true, 1, 2, 3, 4)]
        [InlineData(false, (3d / 2d), true, 1, 1, 1, 100)]
        [InlineData(true, (3d / 2d), false, 1, 1, 1, 100)]
        [InlineData(true, (3d / 2d), true, 8, 8, 8, 8)]
        [InlineData(true, (3d / 2d), true, 8, 8, 8, 9)]
        [InlineData(true, (3d / 2d), true, 1, 1, 1, 1)]
        [Theory]
        public void ArePartyLevelsInProperRange(bool requirePartyMemberLevelProximity, double lowestLevelSupportedMultiplier, bool expectedResult, params int[] partyLevels)
        {
            var players = partyLevels.Select((x, i) => {
                return  PlayerTestDataBuilder.BuildPlayer(id: (uint)(i + 1), skills:new Dictionary<Common.Creatures.SkillType, ISkill>()
                {
                    { SkillType.Level,  new Skill(SkillType.Level, 1, (ushort)x) }
                });
            });

            var config = PartyTestDataBuilder.CreateSharedExperienceConfiguration(
                requirePartyMemberLevelProximity: requirePartyMemberLevelProximity,
                lowestLevelSupportedMultipler: lowestLevelSupportedMultiplier
            );
            var inviteService = PartyTestDataBuilder.CreateInviteService(config);
            var party = PartyTestDataBuilder.CreateParty(inviteService, players.ToArray());

            var service = (SharedExperienceService)party.SharedExperienceService;
            Assert.Equal(expectedResult, service.ArePartyLevelsInProperRange());
        }

        [InlineData(true, 30, 1, true, 101, 100, 7)] // x + 1
        [InlineData(true, 30, 1, true, 100, 101, 7)] // y + 1
        [InlineData(true, 30, 1, true, 130, 100, 7)] // x + 30
        [InlineData(true, 30, 1, true, 100, 130, 7)] // y + 30
        [InlineData(true, 30, 1, false, 100, 131, 7)] // x + 31
        [InlineData(true, 30, 1, false, 131, 100, 7)] // y + 31
        [InlineData(true, 30, 1, true, 100, 100, 8)] // z + 1
        [InlineData(true, 30, 1, true, 100, 100, 6)] // z - 1
        [InlineData(true, 30, 1, false, 100, 100, 9)] // z + 2
        [InlineData(true, 30, 1, false, 100, 100, 5)] // z - 2
        [Theory]
        public void ArePartyMembersCloseEnoughToEachOther(bool requirePartyProximity, int maxDistance, int maxVerticalDistance, bool expectedResult, int x, int y, int z)
        {
            var originPlayer = PlayerTestDataBuilder.BuildPlayer(id: 1);
            var distantPlayer = PlayerTestDataBuilder.BuildPlayer(id: 2);
            distantPlayer.Location = new Location((ushort)x, (ushort)y, (byte)z);

            var config = PartyTestDataBuilder.CreateSharedExperienceConfiguration(
                requirePartyProximity: requirePartyProximity,
                maximumPartyDistanceToReceiveExperienceSharing: maxDistance,
                maximumPartyVerticalDistanceToReceiveExperienceSharing: maxVerticalDistance
            );
            var inviteService = PartyTestDataBuilder.CreateInviteService(config);
            var party = PartyTestDataBuilder.CreateParty(inviteService, originPlayer, distantPlayer);

            var service = (SharedExperienceService)party.SharedExperienceService;
            Assert.Equal(expectedResult, service.ArePartyCloseEnoughToEachOther());
        }

        [Fact]
        public void IsEveryMemberActive_ReturnsTrue_WhenAllMembersHaveHealedAnotherRecently()
        {
            var playerOne = PlayerTestDataBuilder.BuildPlayer(id: 1, hp: 99);
            var playerTwo = PlayerTestDataBuilder.BuildPlayer(id: 2, hp: 99);

            var party = PartyTestDataBuilder.CreateParty(null, playerOne, playerTwo);

            var monsterMock = new Mock<IMonster>();
            var damages = new Dictionary<ICreature, ushort>() { };
            monsterMock.Setup(x => x.Damages).Returns(damages.ToImmutableDictionary());

            playerOne.Heal(1, playerTwo);
            playerTwo.Heal(1, playerOne);

            var service = (SharedExperienceService)party.SharedExperienceService;
            Assert.True(service.IsEveryMemberActive(monsterMock.Object));
        }

        [Fact]
        public void IsEveryMemberActive_ReturnsTrue_WhenAllMembersHaveAttackedTheMonsterOrHealedAPartyMember()
        {
            var playerOne = PlayerTestDataBuilder.BuildPlayer(id: 1, hp: 99);
            var playerTwo = PlayerTestDataBuilder.BuildPlayer(id: 2, hp: 99);

            var party = PartyTestDataBuilder.CreateParty(null, playerOne, playerTwo);

            var monsterMock = new Mock<IMonster>();
            var damages = new Dictionary<ICreature, ushort>()
            {
                { playerTwo, 10 } // Player two attacks the monster.
            };
            monsterMock.Setup(x => x.Damages).Returns(damages.ToImmutableDictionary());

            // Player one heals player two.
            playerTwo.Heal(1, playerOne);

            var service = (SharedExperienceService)party.SharedExperienceService;
            Assert.True(service.IsEveryMemberActive(monsterMock.Object));
        }

        [Fact]
        public void IsEveryMemberActive_ReturnsTrue_WhenAllMembersHaveAttackedTheMonster()
        {

            var playerOne = PlayerTestDataBuilder.BuildPlayer(id: 1, hp: 99);
            var playerTwo = PlayerTestDataBuilder.BuildPlayer(id: 2, hp: 99);

            var party = PartyTestDataBuilder.CreateParty(null, playerOne, playerTwo);

            var monsterMock = new Mock<IMonster>();
            var damages = new Dictionary<ICreature, ushort>()
            {
                { playerOne, 10 },
                { playerTwo, 10 }
            };
            monsterMock.Setup(x => x.Damages).Returns(damages.ToImmutableDictionary());


            var service = (SharedExperienceService)party.SharedExperienceService;
            Assert.True(service.IsEveryMemberActive(monsterMock.Object));
        }

        [Fact]
        public void SharedExperienceService_IntegrationTest()
        {
            // Setup the party members (with sanity checks).
            var partyLeader = PlayerTestDataBuilder.BuildPlayer(id: 1, vocation: 1);
            var invitedPlayer = PlayerTestDataBuilder.BuildPlayer(id: 2, vocation: 2);

            // Setup player vocations.
            var vocationOneMock = new Mock<IVocation>();
            vocationOneMock.Setup(x => x.Id).Returns("1");
            vocationOneMock.Setup(x => x.Name).Returns("One");
            vocationOneMock.Setup(x => x.VocationType).Returns((byte)1);

            var vocationTwoMock = new Mock<IVocation>();
            vocationTwoMock.Setup(x => x.Id).Returns("2");
            vocationTwoMock.Setup(x => x.Name).Returns("Two");
            vocationTwoMock.Setup(x => x.VocationType).Returns((byte)2);

            VocationStore.Load(new List<IVocation>()
            {
                vocationOneMock.Object,
                vocationTwoMock.Object
            });

            // Create the party.
            var party = PartyTestDataBuilder.CreateParty(null, partyLeader, invitedPlayer);

            Assert.True(partyLeader.IsInParty);
            Assert.True(invitedPlayer.IsInParty);
            Assert.Equal(2, party.Members.Count);
            Assert.True(party.IsLeader(partyLeader));
            Assert.True(party.IsMember(invitedPlayer));

            // Time to enable shared experience.
            party.SharedExperienceService.ExperienceSharingEnabled = true;

            // Let's create a monster killed 50/50 by the party members.
            var monsterMock = new Mock<IMonster>();
            monsterMock.Setup(x => x.MaxHealthPoints).Returns(100);
            monsterMock.Setup(x => x.HealthPoints).Returns(0);
            monsterMock.Setup(x => x.IsDead).Returns(true);
            monsterMock.Setup(x => x.Experience).Returns(100);
            var damages = new Dictionary<ICreature, ushort>()
            {
                { partyLeader, 50 },
                { invitedPlayer, 50 }
            };
            monsterMock.Setup(x => x.Damages).Returns(damages.ToImmutableDictionary());

            Assert.True(party.SharedExperienceService.CanPartyReceiveSharedExperience(monsterMock.Object));

            // Monster is worth 100 Experience.
            // Party contributed 100% of the kill.
            // Party contains 2 vocations, so +20% bonus experience.
            // Expected Bonus Experience: 20 Experience.
            Assert.Equal(20, party.SharedExperienceService.GetTotalPartyBonusExperience(monsterMock.Object.Experience));
        }
    }
}