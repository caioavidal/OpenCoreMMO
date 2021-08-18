using Moq;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types.Useables;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures.Services;
using NeoServer.Game.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Xunit;

namespace NeoServer.Game.Creatures.Tests.Services
{
    public class PartyShareExperienceServiceTest
    {
        [Fact]
        public void PartyShareExperienceService_ThrowsNullArgumentException_WhenPartyIsNull()
        {
            var config = new Mock<ISharedExperienceConfiguration>().Object;
            Assert.Throws<ArgumentNullException>(() => new SharedExperienceService(null, config));
        }

        [Fact]
        public void PartyShareExperienceService_ThrowsNullArgumentException_WhenConfigurationIsNull()
        {
            var party = new Mock<IParty>().Object;
            Assert.Throws<ArgumentNullException>(() => new SharedExperienceService(party, null));
        }


        [Fact]
        public void IsExperienceSharingTurnedOn_ReturnsTrue_WhenConfiguredToAlwaysBeOn()
        {
            var configMock = new Mock<ISharedExperienceConfiguration>();
            configMock.Setup(x => x.IsSharedExperienceAlwaysOn).Returns(true);
            var partyMock = new Mock<IParty>();
            partyMock.Setup(x => x.Members).Returns(new List<IPlayer>());
            var service = new SharedExperienceService(partyMock.Object, configMock.Object);

            Assert.True(service.IsExperienceSharingTurnedOn());
        }

        [Fact]
        public void IsExperienceSharingTurnedOn_ReturnsTrue_WhenPartySharedExperienceEnabled()
        {
            var configMock = new Mock<ISharedExperienceConfiguration>();

            var partyMock = new Mock<IParty>();
            partyMock.Setup(x => x.Members).Returns(new List<IPlayer>());

            var service = new SharedExperienceService(partyMock.Object, configMock.Object);
            service.ExperienceSharingEnabled = true;

            Assert.True(service.IsExperienceSharingTurnedOn());
        }

        [Fact]
        public void IsExperienceSharingTurnedOn_ReturnsFalse_WhenPartySharedExperienceDisabled()
        {
            var configMock = new Mock<ISharedExperienceConfiguration>();
            var partyMock = new Mock<IParty>();
            partyMock.Setup(x => x.Members).Returns(new List<IPlayer>());
            var service = new SharedExperienceService(partyMock.Object, configMock.Object);

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
            var configMock = new Mock<ISharedExperienceConfiguration>();
            configMock.Setup(x => x.MinimumMonsterExperienceToBeShared).Returns(minimumExperience);

            var partyMock = new Mock<IParty>();
            partyMock.Setup(x => x.Members).Returns(new List<IPlayer>());

            var service = new SharedExperienceService(partyMock.Object, configMock.Object);

            var monsterMock = new Mock<IMonster>();
            monsterMock.Setup(x => x.Experience).Returns(monsterExperience);

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
            var configMock = new Mock<ISharedExperienceConfiguration>();
            configMock.Setup(x => x.RequirePartyMemberLevelProximity).Returns(requirePartyMemberLevelProximity);
            configMock.Setup(x => x.LowestLevelSupportedMultipler).Returns(lowestLevelSupportedMultiplier);

            var partyMembers = partyLevels.Select(lvl =>
            {
                var member = new Mock<IPlayer>();
                member.Setup(x => x.Level).Returns((ushort)lvl);
                return member.Object;
            });

            var partyMock = new Mock<IParty>();
            partyMock.Setup(x => x.Members).Returns(new List<IPlayer>(partyMembers));

            var service = new SharedExperienceService(partyMock.Object, configMock.Object);

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
            var configMock = new Mock<ISharedExperienceConfiguration>();
            configMock.Setup(x => x.RequirePartyProximity).Returns(requirePartyProximity);
            configMock.Setup(x => x.MaximumPartyDistanceToReceiveExperienceSharing).Returns(maxDistance);
            configMock.Setup(x => x.MaximumPartyVerticalDistanceToReceiveExperienceSharing).Returns(maxVerticalDistance);

            var originPlayer = new Mock<IPlayer>();
            originPlayer.Setup(x => x.Location).Returns(new Location(100, 100, 7));

            var distantPlayer = new Mock<IPlayer>();
            distantPlayer.Setup(x => x.Location).Returns(new Location((ushort)x, (ushort)y, (byte)z));

            var partyMembers = new List<IPlayer>()
            {
                originPlayer.Object,
                distantPlayer.Object
            };

            var partyMock = new Mock<IParty>();
            partyMock.Setup(x => x.Members).Returns(partyMembers);

            var service = new SharedExperienceService(partyMock.Object, configMock.Object);

            Assert.Equal(expectedResult, service.ArePartyCloseEnoughToEachOther());
        }

        [Fact]
        public void IsEveryMemberActive_ReturnsTrue_WhenAllMembersHaveHealedAnotherRecently()
        {
            var playerOneMock = new Mock<IPlayer>();
            playerOneMock.Setup(x => x.HealthPoints).Returns(99);
            playerOneMock.Setup(x => x.MaxHealthPoints).Returns(100);

            var playerTwoMock = new Mock<IPlayer>();
            playerTwoMock.Setup(x => x.HealthPoints).Returns(99);
            playerTwoMock.Setup(x => x.MaxHealthPoints).Returns(100);

            playerOneMock.Setup(x => x.Heal(It.IsAny<ushort>(), It.IsAny<ICombatActor>()))
                      .Raises(player => player.OnHeal += It.IsAny<Heal>(), playerOneMock.Object, playerTwoMock.Object, (ushort)1);

            playerTwoMock.Setup(x => x.Heal(It.IsAny<ushort>(), It.IsAny<ICombatActor>()))
                      .Raises(player => player.OnHeal += It.IsAny<Heal>(), playerTwoMock.Object, playerOneMock.Object, (ushort)1);

            var playerOne = playerOneMock.Object;
            var playerTwo = playerTwoMock.Object;

            var partyMembers = new List<IPlayer>()
            {
                playerOne,
                playerTwo
            };

            var partyMock = new Mock<IParty>();
            partyMock.Setup(x => x.Members).Returns(partyMembers);

            var configMock = new Mock<ISharedExperienceConfiguration>();
            configMock.Setup(x => x.RequirePartyMemberParticipation).Returns(true);
            configMock.Setup(x => x.SecondsBetweenHealsToBeConsideredActive).Returns(30);

            var service = new SharedExperienceService(partyMock.Object, configMock.Object);

            var monsterMock = new Mock<IMonster>();
            var damages = new Dictionary<ICreature, ushort>().ToImmutableDictionary();
            monsterMock.Setup(x => x.Damages).Returns(damages);

            // Reduce each others' health.
            playerOne.Attack(playerTwo);
            playerTwo.Attack(playerOne);

            // Heal each other to record a recent heal.
            playerOne.Heal(1, playerTwo);
            playerTwo.Heal(1, playerOne);

            Assert.True(service.IsEveryMemberActive(monsterMock.Object));
        }

        [Fact]
        public void IsEveryMemberActive_ReturnsTrue_WhenAllMembersHaveAttackedTheMonster()
        {
            var playerOne = new Mock<IPlayer>().Object;

            var playerTwoMock = new Mock<IPlayer>();
            playerTwoMock.Setup(x => x.HealthPoints).Returns(99);
            playerTwoMock.Setup(x => x.MaxHealthPoints).Returns(100);


            playerTwoMock.Setup(x => x.Heal(It.IsAny<ushort>(), It.IsAny<ICombatActor>()))
                      .Raises(player => player.OnHeal += It.IsAny<Heal>(), playerTwoMock.Object, playerOne, (ushort)1);

            var playerTwo = playerTwoMock.Object;

            var partyMembers = new List<IPlayer>()
            {
                playerOne,
                playerTwo
            };

            var partyMock = new Mock<IParty>();
            partyMock.Setup(x => x.Members).Returns(partyMembers);

            var configMock = new Mock<ISharedExperienceConfiguration>();
            configMock.Setup(x => x.RequirePartyMemberParticipation).Returns(true);
            configMock.Setup(x => x.SecondsBetweenHealsToBeConsideredActive).Returns(30);

            var service = new SharedExperienceService(partyMock.Object, configMock.Object);

            var monsterMock = new Mock<IMonster>();
            var damages = new Dictionary<ICreature, ushort>()
            {
                { playerTwo, 10 } // Player two attacks the monster.
            }.ToImmutableDictionary();
            monsterMock.Setup(x => x.Damages).Returns(damages);

            // Player one heals player two.
            playerTwo.Heal(1, playerOne);

            Assert.True(service.IsEveryMemberActive(monsterMock.Object));
        }

        [Fact]
        public void IsEveryMemberActive_ReturnsTrue_WhenAllMembersHaveAttackedTheMonsterOrHealedAPartyMember()
        {
            var playerOne = new Mock<IPlayer>().Object;
            var playerTwo = new Mock<IPlayer>().Object;

            var partyMembers = new List<IPlayer>()
            {
                playerOne,
                playerTwo
            };

            var partyMock = new Mock<IParty>();
            partyMock.Setup(x => x.Members).Returns(partyMembers);

            var configMock = new Mock<ISharedExperienceConfiguration>();
            configMock.Setup(x => x.RequirePartyMemberParticipation).Returns(true);
            configMock.Setup(x => x.SecondsBetweenHealsToBeConsideredActive).Returns(30);

            var service = new SharedExperienceService(partyMock.Object, configMock.Object);

            var monsterMock = new Mock<IMonster>();
            var damages = new Dictionary<ICreature, ushort>()
            {
                { playerOne, 10 },
                { playerTwo, 10 }
            }.ToImmutableDictionary();
            monsterMock.Setup(x => x.Damages).Returns(damages);

            Assert.True(service.IsEveryMemberActive(monsterMock.Object));
        }
    }
}