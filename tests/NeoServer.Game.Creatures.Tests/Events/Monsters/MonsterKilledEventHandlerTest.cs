using Moq;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Creatures.Events.Monsters;
using NeoServer.Game.Creatures.Monsters;
using NeoServer.Game.Creatures.Vocations;
using NeoServer.Game.Tests.Helpers;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Xunit;

namespace NeoServer.Game.Creatures.Tests.Events.Monsters
{
    public class MonsterKilledEventHandlerTest
    {
        [Fact]
        public void Execute_GrantsMonsterExperience_WhenMonsterKilledByOnePlayer()
        {
            var player = PlayerTestDataBuilder.BuildPlayer(id: 1);
            var damages = new Dictionary<ICreature, ushort>()
            {
                { player, 100 }
            };

            var monsterMock = new Mock<IMonster>();
            monsterMock.Setup(x => x.Experience).Returns(100);
            monsterMock.Setup(x => x.Damages).Returns(damages.ToImmutableDictionary());

            var eventHandler = new MonsterKilledEventHandler();

            var before = player.Experience;
            eventHandler.Execute(monsterMock.Object, player, null);
            var after = player.Experience;

            Assert.Equal(before + 100, after);
        }

        [Fact]
        public void Execute_GrantsProportionalMonsterExperience_WhenMonsterKilledByTwoPlayers()
        {
            var playerOne = PlayerTestDataBuilder.BuildPlayer(id: 1);
            var playerTwo = PlayerTestDataBuilder.BuildPlayer(id: 2);
            var damages = new Dictionary<ICreature, ushort>()
            {
                { playerOne, 100 },
                { playerTwo, 100 }
            };

            var monsterMock = new Mock<IMonster>();
            monsterMock.Setup(x => x.Experience).Returns(100);
            monsterMock.Setup(x => x.Damages).Returns(damages.ToImmutableDictionary());

            var eventHandler = new MonsterKilledEventHandler();

            var playerOneBefore = playerOne.Experience;
            var playerTwoBefore = playerTwo.Experience;
            eventHandler.Execute(monsterMock.Object, playerOne, null);
            var playerOneAfter = playerOne.Experience;
            var playerTwoAfter = playerTwo.Experience;

            Assert.Equal(playerOneBefore + 50, playerOneAfter);
            Assert.Equal(playerTwoBefore + 50, playerTwoAfter);
        }

        [Fact]
        public void Execute_ConsidersSummonDamageAsMasterDamage_WhenCalculatingExperienceProportionally()
        {
            var playerOne = PlayerTestDataBuilder.BuildPlayer(id: 1);
            var playerTwo = PlayerTestDataBuilder.BuildPlayer(id: 2);
            var playerOneSummon = MockSummon(playerOne);
            var damages = new Dictionary<ICreature, ushort>()
            {
                { playerOne, 100 },
                { playerTwo, 100 },
                { playerOneSummon, 200 } // This should be considered playerOne's damage.
            };

            var monsterMock = new Mock<IMonster>();
            monsterMock.Setup(x => x.Experience).Returns(100);
            monsterMock.Setup(x => x.Damages).Returns(damages.ToImmutableDictionary());

            var eventHandler = new MonsterKilledEventHandler();

            var playerOneBefore = playerOne.Experience;
            var playerTwoBefore = playerTwo.Experience;
            eventHandler.Execute(monsterMock.Object, playerOne, null);
            var playerOneAfter = playerOne.Experience;
            var playerTwoAfter = playerTwo.Experience;

            Assert.Equal(playerOneBefore + 75, playerOneAfter);
            Assert.Equal(playerTwoBefore + 25, playerTwoAfter);
        }

        [Fact]
        public void Execute_GrantsProportionalMonsterExperience_WhenSharedExperienceIsDisabled()
        {
            var playerOne = PlayerTestDataBuilder.BuildPlayer(id: 1);
            var playerTwo = PlayerTestDataBuilder.BuildPlayer(id: 2);
            var party = PartyTestDataBuilder.CreateParty(null, playerOne, playerTwo);
            var damages = new Dictionary<ICreature, ushort>()
            {
                { playerOne, 300 },
                { playerTwo, 100 },
            };

            var monsterMock = new Mock<IMonster>();
            monsterMock.Setup(x => x.Experience).Returns(100);
            monsterMock.Setup(x => x.Damages).Returns(damages.ToImmutableDictionary());

            var eventHandler = new MonsterKilledEventHandler();

            var playerOneBefore = playerOne.Experience;
            var playerTwoBefore = playerTwo.Experience;
            eventHandler.Execute(monsterMock.Object, playerOne, null);
            var playerOneAfter = playerOne.Experience;
            var playerTwoAfter = playerTwo.Experience;

            Assert.Equal(playerOneBefore + 75, playerOneAfter);
            Assert.Equal(playerTwoBefore + 25, playerTwoAfter);
        }

        [Fact]
        public void Execute_GrantsProportionalMonsterExperienceAndBonus_WhenSharedExperienceIsEnabled()
        {
            // p1 and p2 will be in a party. p3 will be solo.
            var playerOne = PlayerTestDataBuilder.BuildPlayer(id: 1, vocation: 1);
            var playerTwo = PlayerTestDataBuilder.BuildPlayer(id: 2, vocation: 2);
            var playerThree = PlayerTestDataBuilder.BuildPlayer(id: 3, vocation: 3);

            MockVocations(1, 2);

            var party = PartyTestDataBuilder.CreateParty(null, playerOne, playerTwo);
            party.SharedExperienceService.ExperienceSharingEnabled = true;

            var damages = new Dictionary<ICreature, ushort>()
            {
                // The party was 3/4 of the overall contribution, so the party gets 75 monster experience + 15 bonus (+20% from two vocations).
                // Player one was 2/3 of the party contribution, so they will get 60 exp.
                // Player two was 1/3 of the party contribution, so they will get 30 exp.
                // Player three was 1/4 of the overall contribution, so they will get 25 exp.
                { playerOne, 200 },
                { playerTwo, 100 },
                { playerThree, 100 }
            };

            var monsterMock = new Mock<IMonster>();
            monsterMock.Setup(x => x.Experience).Returns(100);
            monsterMock.Setup(x => x.Damages).Returns(damages.ToImmutableDictionary());

            var eventHandler = new MonsterKilledEventHandler();

            var playerOneBefore = playerOne.Experience;
            var playerTwoBefore = playerTwo.Experience;
            var playerThreeBefore = playerThree.Experience;
            eventHandler.Execute(monsterMock.Object, playerOne, null);
            var playerOneAfter = playerOne.Experience;
            var playerTwoAfter = playerTwo.Experience;
            var playerThreeAfter = playerThree.Experience;

            Assert.Equal(playerOneBefore + 60, playerOneAfter);
            Assert.Equal(playerTwoBefore + 30, playerTwoAfter);
            Assert.Equal(playerThreeBefore + 25, playerThreeAfter);
        }

        /// <summary>
        /// Mocks a summon for the specified player.
        /// </summary>
        /// <param name="player">The master of the summon.</param>
        private Summon MockSummon(IPlayer player)
        {
            var monsterTypeMock = new Mock<IMonsterType>();
            monsterTypeMock.Setup(x => x.Name).Returns("Test Summon");
            monsterTypeMock.Setup(x => x.Look).Returns(new Dictionary<LookType, ushort>()
            {
                { LookType.Type, 0 },
                { LookType.Body, 0 },
                { LookType.Feet, 0 },
                { LookType.Head, 0 },
                { LookType.Legs, 0 }
            });

            return new Summon(monsterTypeMock.Object, player);
        }

        /// <summary>
        /// Mocks and loads a vocation matching each number (VocationType byte).
        /// </summary>
        private void MockVocations(params int[] vocations)
        {
            var mockedVocations = vocations.Select(x =>
            {
                var mock = new Mock<IVocation>();
                mock.Setup(x => x.Id).Returns(x.ToString());
                mock.Setup(x => x.Name).Returns(x.ToString());
                mock.Setup(x => x.VocationType).Returns((byte)x);
                return mock.Object;
            });

            VocationStore.Load(mockedVocations);
        }
    }
}