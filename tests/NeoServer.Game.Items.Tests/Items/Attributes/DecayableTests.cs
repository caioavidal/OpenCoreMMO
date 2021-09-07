using System.Threading;
using FluentAssertions;
using NeoServer.Game.Items.Items.Attributes;
using NeoServer.Game.Tests.Helpers;
using Xunit;

namespace NeoServer.Game.Items.Tests.Items.Attributes
{
    public class DecayableTests
    {
        [Fact]
        public void Expired_DidNotStartToDecay_ReturnsFalse()
        {
            //arrange
            var item = ItemTestData.CreateRing(1);
            var sut = new Decayable(item, ()=>item.Metadata,60);

            //assert
            sut.Expired.Should().BeFalse();
        }
        [Fact]
        public void Expired_ElapsedLessThanDuration_ReturnsFalse()
        {
            //arrange
            var item = ItemTestData.CreateRing(1);
            var sut = new Decayable(item, () => item.Metadata, 60);

            //act
            sut.StartDecay();
            Thread.Sleep(2000);
            sut.PauseDecay();
            //assert
            sut.Expired.Should().BeFalse();
        }
        [Fact]
        public void Expired_ElapsedGreaterThanDuration_ReturnsTrue()
        {
            //arrange
            var item = ItemTestData.CreateRing(1);
            var sut = new Decayable(item, () => item.Metadata, 2);

            //act
            sut.StartDecay();
            Thread.Sleep(2500);
            sut.PauseDecay();
            //assert
            sut.Expired.Should().BeTrue();
        }

        [Fact]
        public void StartedToDecay_CalledStart_ReturnsTrue()
        {
            //arrange
            var item = ItemTestData.CreateRing(1);
            var sut = new Decayable(item, () => item.Metadata, 2);

            //act
            sut.StartDecay();
            Thread.Sleep(2500);
            sut.PauseDecay();
            //assert
            sut.StartedToDecay.Should().BeTrue();
        }
        [Fact]
        public void StartedToDecay_DidNotStart_ReturnsFalse()
        {
            //arrange
            var item = ItemTestData.CreateRing(1);
            var sut = new Decayable(item, () => item.Metadata, 2);
            
            //assert
            sut.StartedToDecay.Should().BeFalse();
        }
        [Fact]
        public void Start_IncreaseElapsedTime()
        {
            //arrange
            var item = ItemTestData.CreateRing(1);
            var sut = new Decayable(item, () => item.Metadata, 20);

            //act
            sut.StartDecay();
            Thread.Sleep(1000);
            sut.PauseDecay();

            //assert
            var elapsed = sut.Elapsed;
            sut.Elapsed.Should().BeGreaterThan(0);

            //act
            sut.StartDecay();
            Thread.Sleep(1000);
            sut.PauseDecay();

            //assert
            sut.Elapsed.Should().BeGreaterThan(elapsed);
        }
        [Fact]
        public void ShouldDisappear_DecaysToNull_ReturnsTrue()
        {
            //arrange
            var item = ItemTestData.CreateRing(1);
            var sut = new Decayable(item, () => null, 20);

            //assert
            sut.ShouldDisappear.Should().BeTrue();
        }
        [Fact]
        public void ShouldDisappear_DecaysTo10_ReturnsFalse()
        {
            //arrange
            var item = ItemTestData.CreateRing(1);

            var decayToItem = ItemTestData.CreateRing(10);

            var sut = new Decayable(item, () => decayToItem.Metadata, 20);

            //assert
            sut.ShouldDisappear.Should().BeFalse();
        }

        [Fact]
        public void Elapsed_2Secs_Returns2()
        {
            //arrange
            var item = ItemTestData.CreateRing(1);
            var decayToItem = ItemTestData.CreateRing(10);

            var sut = new Decayable(item, () => decayToItem.Metadata, 20);

            //act
            sut.StartDecay();
            Thread.Sleep(2000);
            sut.PauseDecay();

            //assert
            sut.Elapsed.Should().Be(2);
        }
        [Fact]
        public void Elapsed_AfterPause_DoNotChange()
        {
            //arrange
            var item = ItemTestData.CreateRing(1);
            var decayToItem = ItemTestData.CreateRing(10);
            var sut = new Decayable(item, () => decayToItem.Metadata, 20);

            //act
            sut.StartDecay();
            Thread.Sleep(2000);
            sut.PauseDecay();
            Thread.Sleep(2000);

            //assert
            sut.Elapsed.Should().Be(2);
        }

        [Fact]
        public void Remaining_DidNotStarted_ReturnsDuration()
        {
            //arrange
            var item = ItemTestData.CreateRing(1);
            var decayToItem = ItemTestData.CreateRing(10);

            var sut = new Decayable(item, () => decayToItem.Metadata, 20);
            
            //assert
            sut.Remaining.Should().Be(20);
        }
        [Fact]
        public void Remaining_Elapsed2SecsOf20_Returns18()
        {
            //arrange
            var item = ItemTestData.CreateRing(1);
            var decayToItem = ItemTestData.CreateRing(10);

            var sut = new Decayable(item, () => decayToItem.Metadata, 20);

            //act
            sut.StartDecay();
            Thread.Sleep(2000);
            sut.PauseDecay();

            //assert
            sut.Remaining.Should().Be(18);
        }
        [Fact]
        public void Remaining_Elapsed2SecsOf2_Returns0()
        {
            //arrange
            var item = ItemTestData.CreateRing(1);
            var decayToItem = ItemTestData.CreateRing(10);

            var sut = new Decayable(item, () => decayToItem.Metadata, 2);

            //act
            sut.StartDecay();
            Thread.Sleep(2000);
            sut.PauseDecay();

            //assert
            sut.Remaining.Should().Be(0);
        }
        [Fact]
        public void Pause_EmitEvent()
        {
            //arrange
            var item = ItemTestData.CreateRing(1);
            var decayToItem = ItemTestData.CreateRing(10);

            var sut = new Decayable(item, () => decayToItem.Metadata, 2);

            var emitted = false;
            sut.OnPaused += _ => emitted = true;
            //act
            sut.StartDecay();
            Thread.Sleep(2000);
            sut.PauseDecay();

            //assert
            emitted.Should().BeTrue();
        }
        [Fact]
        public void Start_EmitEvent()
        {
            //arrange
            var item = ItemTestData.CreateRing(1);
            var decayToItem = ItemTestData.CreateRing(10);

            var sut = new Decayable(item, () => decayToItem.Metadata, 2);

            var emitted = false;
            sut.OnStarted += _ => emitted = true;
            //act
            sut.StartDecay();
            //assert
            emitted.Should().BeTrue();
        }
    }
}
