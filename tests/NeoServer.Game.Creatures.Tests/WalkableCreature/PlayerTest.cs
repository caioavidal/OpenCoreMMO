using System.Collections.Generic;
using Moq;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures.Model.Players;
using NeoServer.Game.DataStore;
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
            var directions = new[] { Direction.North };
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
            var directions = new[] { Direction.North, Direction.East };
            var pathFinder = new Mock<IPathFinder>();
            pathFinder.Setup(x => x.Find(It.IsAny<ICreature>(), It.IsAny<Location>(), It.IsAny<FindPathParams>(),
                It.IsAny<ITileEnterRule>(), out directions)).Returns(true);

            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100, speed: 300, pathFinder: pathFinder.Object);
            var followEventEmitted = false;
            var walkEventEmitted = false;
            
            sut.OnStartedWalking += _ => walkEventEmitted = true;
            sut.OnStartedFollowing += (_, _, _) => followEventEmitted = true;

            var creature = new Mock<ICreature>();
            creature.Setup(x => x.Location).Returns(new Location(100, 105, 7));
            creature.Setup(x => x.CreatureId).Returns(123);

            var tile = new Mock<IDynamicTile>();
            tile.Setup(x => x.Ground.StepSpeed).Returns(1000);
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

            var directions = new[] { Direction.North, Direction.East };
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
            var directions = new[] { Direction.North, Direction.East };
            var pathFinder = new Mock<IPathFinder>();
            pathFinder.Setup(x => x.Find(It.IsAny<ICreature>(), It.IsAny<Location>(), It.IsAny<FindPathParams>(),
                It.IsAny<ITileEnterRule>(), out directions)).Returns(true);

            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100, speed: 300, pathFinder: pathFinder.Object);

            var stoppedWalkingEvent = false;
            
            sut.OnStoppedWalking += _ => stoppedWalkingEvent = true;

            var tile = new Mock<IDynamicTile>();
            tile.Setup(x => x.Ground.StepSpeed).Returns(100);
            tile.Setup(x => x.Location).Returns(new Location(100, 100, 7));

            sut.SetCurrentTile(tile.Object);
            sut.WalkTo(new Location(102, 100, 7));

            Assert.True(sut.HasNextStep);
            Assert.True(stoppedWalkingEvent);
            Assert.Equal(Direction.North, sut.GetNextStep());
            Assert.Equal(Direction.East, sut.GetNextStep());
        }
    }
}