using Moq;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Tests;
using Xunit;

namespace NeoServer.Game.Creatures.Tests.Players
{
    public partial class PlayerTest
    {
        [Theory]
        [InlineData(Direction.NorthWest, Direction.West)]
        [InlineData(Direction.SouthWest, Direction.West)]
        [InlineData(Direction.NorthEast, Direction.East)]
        [InlineData(Direction.SouthEast, Direction.East)]
        [InlineData(Direction.South, Direction.South)]
        [InlineData(Direction.East, Direction.East)]
        [InlineData(Direction.West, Direction.West)]
        [InlineData(Direction.North, Direction.North)]
        public void SafeDirection_When_Is_Diagonal_Return_Safe_Direction(Direction input, Direction expected)
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100);
            sut.TurnTo(input);
            Assert.Equal(expected, sut.SafeDirection);
        }

        [Fact]
        public void ChangeOutfit_Changes_Outfit_And_Emit_Event()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100);
            bool changedOutfit = false;

            sut.OnChangedOutfit += (a, b) => changedOutfit = true;

            sut.ChangeOutfit(1, 1, 1, 1, 1, 1, 1);

            Assert.Equal(1, sut.Outfit.LookType);
            Assert.Equal(1, sut.Outfit.Addon);
            Assert.Equal(1, sut.Outfit.Body);
            Assert.Equal(1, sut.Outfit.Feet);
            Assert.Equal(1, sut.Outfit.Head);
            Assert.Equal(1, sut.Outfit.Id);
            Assert.Equal(1, sut.Outfit.Legs);
            Assert.True(changedOutfit);
        }

        [Fact]
        public void SetTemporaryOutfit_Store_Current_To_LastOutfit_And_Changes_Outfit()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100);
            bool changedOutfit = false;

            sut.OnChangedOutfit += (a, b) => changedOutfit = true;

            sut.SetTemporaryOutfit(1, 1, 1, 1, 1, 1, 1);

            Assert.Equal(1, sut.Outfit.LookType);
            Assert.Equal(1, sut.Outfit.Addon);
            Assert.Equal(1, sut.Outfit.Body);
            Assert.Equal(1, sut.Outfit.Feet);
            Assert.Equal(1, sut.Outfit.Head);
            Assert.Equal(1, sut.Outfit.Id);
            Assert.Equal(1, sut.Outfit.Legs);
            Assert.True(changedOutfit);

            Assert.Equal(0, sut.LastOutfit.LookType);
            Assert.Equal(0, sut.LastOutfit.Addon);
            Assert.Equal(0, sut.LastOutfit.Body);
            Assert.Equal(0, sut.LastOutfit.Feet);
            Assert.Equal(0, sut.LastOutfit.Head);
            Assert.Equal(0, sut.LastOutfit.Id);
            Assert.Equal(0, sut.LastOutfit.Legs);
        }

        [Fact]
        public void BackToOldOutfit_Sets_LastOutfit_To_Outfit_And_Changes_Outfit()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100);
            bool changedOutfit = false;

            sut.SetTemporaryOutfit(1, 1, 1, 1, 1, 1, 1);

            sut.OnChangedOutfit += (a, b) => changedOutfit = true;

            sut.BackToOldOutfit();

            Assert.Null(sut.LastOutfit);
            Assert.True(changedOutfit);

            Assert.Equal(0, sut.Outfit.LookType);
            Assert.Equal(0, sut.Outfit.Addon);
            Assert.Equal(0, sut.Outfit.Body);
            Assert.Equal(0, sut.Outfit.Feet);
            Assert.Equal(0, sut.Outfit.Head);
            Assert.Equal(0, sut.Outfit.Id);
            Assert.Equal(0, sut.Outfit.Legs);
        }

        [Fact]
        public void CanSeeInvisible_Returns_Flag_Value()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100);

            Assert.False(sut.CanSeeInvisible);

            sut.SetFlag(PlayerFlag.CanSeeInvisibility);

            Assert.True(sut.CanSeeInvisible);
        }

        [Fact]
        public void CanSee_When_Creature_Is_Invisible_And_Cant_See_Invisible_Returns_False()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100);

            var creature = new Mock<ICreature>();
            creature.Setup(x => x.IsInvisible).Returns(true);

            var result = sut.CanSee(creature.Object);

            Assert.False(result);
        }
        [Fact]
        public void CanSee_When_Creature_Is_Invisible_And_Can_See_Invisible_Returns_True()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100);

            sut.SetFlag(PlayerFlag.CanSeeInvisibility);

            var creature = new Mock<ICreature>();
            creature.Setup(x => x.IsInvisible).Returns(true);

            var result = sut.CanSee(creature.Object);

            Assert.True(result);
        }
        [Fact]

        public void Say_Should_Emit_Event()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100);
            var messageEmitted = "";
            var speechTypeEmitted = SpeechType.None;

            sut.SetTemporaryOutfit(1, 1, 1, 1, 1, 1, 1);

            sut.OnSay += (creature, type, message, receiver) =>
            {
                messageEmitted = message;
                speechTypeEmitted = type;
            };

            sut.Say("Hello", SpeechType.Say);

            Assert.Equal("Hello", messageEmitted);
            Assert.Equal(SpeechType.Say, speechTypeEmitted);
        }
        [Fact]
        public void Say_To_Receiver_Should_Emit_Event()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100);
            var receiver = new Mock<ICreature>();
            var messageEmitted = "";
            var speechTypeEmitted = SpeechType.None;
            ICreature to = null;

            sut.SetTemporaryOutfit(1, 1, 1, 1, 1, 1, 1);

            sut.OnSay += (creature, type, message, receiver) =>
            {
                messageEmitted = message;
                speechTypeEmitted = type;
                to = receiver;
            };

            sut.Say("Hello", SpeechType.Private, receiver.Object);

            Assert.Equal("Hello", messageEmitted);
            Assert.Equal(SpeechType.Private, speechTypeEmitted);
            Assert.Equal(receiver.Object, to);
        }
        [Fact]
        public void Say_Empty_Message_Dont_Emit_Event()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100);
            var receiver = new Mock<ICreature>();
            string messageEmitted = null;
            var speechTypeEmitted = SpeechType.None;
            ICreature to = null;

            sut.SetTemporaryOutfit(1, 1, 1, 1, 1, 1, 1);

            sut.OnSay += (creature, type, message, receiver) =>
            {
                messageEmitted = message;
                speechTypeEmitted = type;
                to = receiver;
            };

            sut.Say("", SpeechType.Private, receiver.Object);

            Assert.Null(messageEmitted);
            Assert.Equal(SpeechType.None, speechTypeEmitted);
            Assert.Null(to);
        }

        [Fact]
        public void CanBeSeen_Returns_True_Or_False_Depending_On_Flag_State()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100);

            sut.SetFlag(PlayerFlag.CanBeSeen);
            Assert.True(sut.CanBeSeen);

            sut.UnsetFlag(PlayerFlag.CanBeSeen);
            Assert.False(sut.CanBeSeen);
        }

    }
}
