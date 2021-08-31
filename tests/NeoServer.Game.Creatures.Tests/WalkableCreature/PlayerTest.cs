using System;
using System.Collections.Generic;
using Moq;
using NeoServer.Extensions.Runes;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures.Model.Players;
using NeoServer.Game.DataStore;
using NeoServer.Game.Items.Items.UsableItems.Runes;
using NeoServer.Game.Tests;
using NeoServer.Game.Tests.Helpers;
using Xunit;

namespace NeoServer.Game.Creatures.Tests.WalkableCreature
{
    public class PlayerTest
    {
        [Fact]
        public void HasNextStep_Returns_True_When_Player_Has_Steps_To_Walk()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100, skills: new Dictionary<SkillType, ISkill>
            {
                {SkillType.Level, new Skill(SkillType.Level, 1.1f, 100)}
            });

            Assert.False(sut.HasNextStep);
            sut.WalkTo(Direction.South, Direction.North);
            Assert.True(sut.HasNextStep);
        }

        [Fact]
        public void IsFollowing_Returns_True_When_Player_Is_Following_Someone()
        {
            var pathFinder = new Mock<IPathFinder>();
            var directions = new[] {Direction.North};
            pathFinder.Setup(x => x.Find(It.IsAny<ICreature>(), It.IsAny<Location>(), It.IsAny<FindPathParams>(),
                It.IsAny<ITileEnterRule>(), out directions)).Returns(true);
            GameToolStore.PathFinder = pathFinder.Object;

            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100, skills: new Dictionary<SkillType, ISkill>
            {
                {SkillType.Level, new Skill(SkillType.Level, 1.1f, 100)}
            });

            var creature = new Mock<ICreature>();
            creature.Setup(x => x.Location).Returns(sut.Location);
            creature.Setup(x => x.CreatureId).Returns(123);

            Assert.False(sut.IsFollowing);
            sut.Follow(creature.Object);
            Assert.True(sut.IsFollowing);
        }

        [Theory]
        [InlineData(100, 200)]
        [InlineData(300, 0)]
        [InlineData(400, 0)]
        public void DecreaseSpeed_Should_Decrease_Speed_Value(ushort decrease, ushort expected)
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100, speed: 300);
            var emittedEvent = false;
            sut.OnChangedSpeed += (creature, speed) => emittedEvent = true;

            sut.DecreaseSpeed(decrease);

            Assert.Equal(expected, sut.Speed);
            Assert.True(emittedEvent);
        }

        [Theory]
        [InlineData(100, 400)]
        [InlineData(0, 300)]
        [InlineData(300, 600)]
        public void IncreaseSpeed_Should_Increase_Speed_Value(ushort increase, ushort expected)
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100, speed: 300);
            var emittedEvent = false;
            sut.OnChangedSpeed += (creature, speed) => emittedEvent = true;

            sut.IncreaseSpeed(increase);

            Assert.Equal(expected, sut.Speed);
            Assert.True(emittedEvent);
        }

        [Fact]
        public void Follow_Should_Emmit_Follow_And_Walk_Event()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100, speed: 300);
            var followEventEmitted = false;
            var walkEventEmitted = false;

            var directions = new[] {Direction.North, Direction.East};
            var pathFinder = new Mock<IPathFinder>();
            pathFinder.Setup(x => x.Find(It.IsAny<ICreature>(), It.IsAny<Location>(), It.IsAny<FindPathParams>(),
                It.IsAny<ITileEnterRule>(), out directions)).Returns(true);
            GameToolStore.PathFinder = pathFinder.Object;

            sut.OnStartedWalking += creature => walkEventEmitted = true;
            sut.OnStartedFollowing += (creature, to, fpp) => followEventEmitted = true;

            var creature = new Mock<ICreature>();
            creature.Setup(x => x.Location).Returns(new Location(100, 105, 7));
            creature.Setup(x => x.CreatureId).Returns(123);

            var tile = new Mock<IDynamicTile>();
            tile.Setup(x => x.Ground.StepSpeed).Returns(100);
            tile.Setup(x => x.Location).Returns(new Location(100, 100, 7));

            sut.SetCurrentTile(tile.Object);
            sut.Follow(creature.Object);

            Assert.True(sut.IsFollowing);
            Assert.Equal(creature.Object, sut.Following);
            Assert.True(followEventEmitted);
            Assert.True(walkEventEmitted);
            Assert.Equal(Direction.North, sut.GetNextStep());
            Assert.Equal(Direction.East, sut.GetNextStep());
        }

        [Fact]
        public void StopFollowing_Should_Stop_Following()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100, speed: 300);
            var stoppedWalkEventEmitted = false;

            var directions = new[] {Direction.North, Direction.East};
            var pathFinder = new Mock<IPathFinder>();
            pathFinder.Setup(x => x.Find(It.IsAny<ICreature>(), It.IsAny<Location>(), It.IsAny<FindPathParams>(),
                It.IsAny<ITileEnterRule>(), out directions)).Returns(true);
            GameToolStore.PathFinder = pathFinder.Object;

            sut.OnStoppedWalking += creature => stoppedWalkEventEmitted = true;

            var creature = new Mock<ICreature>();
            creature.Setup(x => x.Location).Returns(new Location(100, 105, 7));
            creature.Setup(x => x.CreatureId).Returns(123);

            var tile = new Mock<IDynamicTile>();
            tile.Setup(x => x.Ground.StepSpeed).Returns(100);
            tile.Setup(x => x.Location).Returns(new Location(100, 100, 7));

            sut.SetCurrentTile(tile.Object);
            sut.Follow(creature.Object);

            sut.StopFollowing();

            Assert.False(sut.IsFollowing);
            Assert.Null(sut.Following);
            Assert.True(stoppedWalkEventEmitted);
            Assert.Equal(Direction.None, sut.GetNextStep());
        }

        [Fact]
        public void WalkTo_Should_Emit_Events_And_Add_Next_Steps()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100, speed: 300);
            var startedWalkingEvent = false;

            var directions = new[] {Direction.North, Direction.East};
            var pathFinder = new Mock<IPathFinder>();
            pathFinder.Setup(x => x.Find(It.IsAny<ICreature>(), It.IsAny<Location>(), It.IsAny<FindPathParams>(),
                It.IsAny<ITileEnterRule>(), out directions)).Returns(true);
            GameToolStore.PathFinder = pathFinder.Object;

            sut.OnStoppedWalking += creature => startedWalkingEvent = true;

            var tile = new Mock<IDynamicTile>();
            tile.Setup(x => x.Ground.StepSpeed).Returns(100);
            tile.Setup(x => x.Location).Returns(new Location(100, 100, 7));

            sut.SetCurrentTile(tile.Object);
            sut.WalkTo(new Location(102, 100, 7));

            Assert.True(sut.HasNextStep);
            Assert.True(startedWalkingEvent);
            Assert.Equal(Direction.North, sut.GetNextStep());
            Assert.Equal(Direction.East, sut.GetNextStep());
        }

        [Fact]
        public void PlayerDoesNotGainSkillsWhenUsingAnAttackRune()
        {
            var player = PlayerTestDataBuilder.BuildPlayer();
            var targetPlayer = PlayerTestDataBuilder.BuildPlayer();

            var itemAttirbuteListMock = new Mock<IItemAttributeList>();

            itemAttirbuteListMock
                .Setup(x => x.GetAttribute<bool>(It.IsAny<ItemAttribute>()))
                .Returns(true);

            itemAttirbuteListMock
                .Setup(x => x.GetAttributeArray(It.IsAny<string>()))
                .Returns(new dynamic[] { "0.0", "0.0" });

            var itemTypeMock = new Mock<IItemType>();
            itemTypeMock
                .Setup(x => x.Attributes)
                .Returns(itemAttirbuteListMock.Object);

            var rune = new AttackRune(itemTypeMock.Object, new Location(100, 100, 7), (byte)10);

            var before = new Dictionary<SkillType, byte>()
            {
                { SkillType.Axe, player.GetSkillTries(SkillType.Axe) },
                { SkillType.Club, player.GetSkillTries(SkillType.Club) },
                { SkillType.Distance, player.GetSkillTries(SkillType.Distance) },
                { SkillType.Fishing, player.GetSkillTries(SkillType.Fishing) },
                { SkillType.Fist, player.GetSkillTries(SkillType.Fist) },
                { SkillType.Level, player.GetSkillTries(SkillType.Level) },
                { SkillType.Magic, player.GetSkillTries(SkillType.Magic) },
                { SkillType.Shielding, player.GetSkillTries(SkillType.Shielding) },
                { SkillType.Speed, player.GetSkillTries(SkillType.Speed) },
                { SkillType.Sword, player.GetSkillTries(SkillType.Sword) },
            };

            var result = rune.Use(player, targetPlayer, out var attackType);
            Assert.True(result);

            var after = new Dictionary<SkillType, byte>()
            {
                { SkillType.Axe, player.GetSkillTries(SkillType.Axe) },
                { SkillType.Club, player.GetSkillTries(SkillType.Club) },
                { SkillType.Distance, player.GetSkillTries(SkillType.Distance) },
                { SkillType.Fishing, player.GetSkillTries(SkillType.Fishing) },
                { SkillType.Fist, player.GetSkillTries(SkillType.Fist) },
                { SkillType.Level, player.GetSkillTries(SkillType.Level) },
                { SkillType.Magic, player.GetSkillTries(SkillType.Magic) },
                { SkillType.Shielding, player.GetSkillTries(SkillType.Shielding) },
                { SkillType.Speed, player.GetSkillTries(SkillType.Speed) },
                { SkillType.Sword, player.GetSkillTries(SkillType.Sword) },
            };

            foreach (var skillType in before.Keys)
            {
                Assert.Equal(before[skillType], after[skillType]);
            }
        }

        [Fact]
        public void PlayerDoesNotGainSkillsWhenUsingAHealingRune()
        {
            var player = PlayerTestDataBuilder.BuildPlayer();
            var targetPlayer = PlayerTestDataBuilder.BuildPlayer();

            var itemAttirbuteListMock = new Mock<IItemAttributeList>();

            itemAttirbuteListMock
                .Setup(x => x.GetAttribute<bool>(It.IsAny<ItemAttribute>()))
                .Returns(true);

            itemAttirbuteListMock
                .Setup(x => x.GetAttributeArray(It.IsAny<string>()))
                .Returns(new dynamic[] { "0.0", "0.0" });

            var itemTypeMock = new Mock<IItemType>();
            itemTypeMock
                .Setup(x => x.Attributes)
                .Returns(itemAttirbuteListMock.Object);

            var rune = new HealingRune(itemTypeMock.Object, new Location(100, 100, 7), new Dictionary<ItemAttribute, IConvertible>());

            var before = new Dictionary<SkillType, byte>()
            {
                { SkillType.Axe, player.GetSkillTries(SkillType.Axe) },
                { SkillType.Club, player.GetSkillTries(SkillType.Club) },
                { SkillType.Distance, player.GetSkillTries(SkillType.Distance) },
                { SkillType.Fishing, player.GetSkillTries(SkillType.Fishing) },
                { SkillType.Fist, player.GetSkillTries(SkillType.Fist) },
                { SkillType.Level, player.GetSkillTries(SkillType.Level) },
                { SkillType.Magic, player.GetSkillTries(SkillType.Magic) },
                { SkillType.Shielding, player.GetSkillTries(SkillType.Shielding) },
                { SkillType.Speed, player.GetSkillTries(SkillType.Speed) },
                { SkillType.Sword, player.GetSkillTries(SkillType.Sword) },
            };

            rune.Use(player, targetPlayer);

            var after = new Dictionary<SkillType, byte>()
            {
                { SkillType.Axe, player.GetSkillTries(SkillType.Axe) },
                { SkillType.Club, player.GetSkillTries(SkillType.Club) },
                { SkillType.Distance, player.GetSkillTries(SkillType.Distance) },
                { SkillType.Fishing, player.GetSkillTries(SkillType.Fishing) },
                { SkillType.Fist, player.GetSkillTries(SkillType.Fist) },
                { SkillType.Level, player.GetSkillTries(SkillType.Level) },
                { SkillType.Magic, player.GetSkillTries(SkillType.Magic) },
                { SkillType.Shielding, player.GetSkillTries(SkillType.Shielding) },
                { SkillType.Speed, player.GetSkillTries(SkillType.Speed) },
                { SkillType.Sword, player.GetSkillTries(SkillType.Sword) },
            };

            foreach (var skillType in before.Keys)
            {
                Assert.Equal(before[skillType], after[skillType]);
            }
        }
    }
}