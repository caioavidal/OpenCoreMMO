using Moq;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.DataStore;
using NeoServer.Game.Tests;
using NeoServer.Game.World.Map.Tiles;
using NeoServer.Server.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NeoServer.Game.Creatures.Tests.WalkableCreature
{
    public partial class PlayerTest
    {
        [Fact]
        public void HasNextStep_Returns_True_When_Player_Has_Steps_To_Walk()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100, skills: new Dictionary<SkillType, ISkill>
              {
                    { SkillType.Level, new Skill(SkillType.Level, 1.1f,100,0)  }

              });


            Assert.False(sut.HasNextStep);
            sut.WalkTo(Direction.South, Direction.North);
            Assert.True(sut.HasNextStep);
        }

        [Fact]
        public void IsFollowing_Returns_True_When_Player_Is_Following_Someone()
        {
            var pathFinder = new Mock<IPathFinder>();
            var directions = new Direction[] { Direction.North };
            pathFinder.Setup(x => x.Find(It.IsAny<ICreature>(), It.IsAny<Location>(), It.IsAny<FindPathParams>(), It.IsAny<ITileEnterRule>(), out directions)).Returns(true);
            ConfigurationStore.PathFinder = pathFinder.Object;

            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100, skills: new Dictionary<SkillType, ISkill>
              {
                    { SkillType.Level, new Skill(SkillType.Level, 1.1f,100,0)  }
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

            var directions = new Direction[] { Direction.North, Direction.East };
            var pathFinder = new Mock<IPathFinder>();
            pathFinder.Setup(x => x.Find(It.IsAny<ICreature>(), It.IsAny<Location>(), It.IsAny<FindPathParams>(), It.IsAny<ITileEnterRule>(), out directions)).Returns(true);
            ConfigurationStore.PathFinder = pathFinder.Object;

            sut.OnStartedWalking += (creature) => walkEventEmitted = true;
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
            Assert.Equal(123u, sut.Following);
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

            var directions = new Direction[] { Direction.North, Direction.East };
            var pathFinder = new Mock<IPathFinder>();
            pathFinder.Setup(x => x.Find(It.IsAny<ICreature>(), It.IsAny<Location>(), It.IsAny<FindPathParams>(), It.IsAny<ITileEnterRule>(), out directions)).Returns(true);
            ConfigurationStore.PathFinder = pathFinder.Object;

            sut.OnStoppedWalking += (creature) => stoppedWalkEventEmitted = true;

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
            Assert.Equal(0u, sut.Following);
            Assert.True(stoppedWalkEventEmitted);
            Assert.Equal(Direction.None, sut.GetNextStep());
        }

        [Fact]
        public void WalkTo_Should_Emit_Events_And_Add_Next_Steps()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100, speed: 300);
            var startedWalkingEvent = false;

            var directions = new Direction[] { Direction.North, Direction.East };
            var pathFinder = new Mock<IPathFinder>();
            pathFinder.Setup(x => x.Find(It.IsAny<ICreature>(), It.IsAny<Location>(), It.IsAny<FindPathParams>(), It.IsAny<ITileEnterRule>(), out directions)).Returns(true);
            ConfigurationStore.PathFinder = pathFinder.Object;

            sut.OnStoppedWalking += (creature) => startedWalkingEvent = true;

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


    }
}
