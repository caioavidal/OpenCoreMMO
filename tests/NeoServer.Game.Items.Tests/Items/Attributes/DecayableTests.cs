using System.Threading;
using FluentAssertions;
using NeoServer.Game.Common.Contracts.Items;
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
            var item = ItemTestData.CreateDefenseEquipmentItem(1);
            var sut = new Decayable(() => item.Metadata, 60);

            //assert
            sut.Expired.Should().BeFalse();
        }
        [Fact]
        public void Expired_ElapsedLessThanDuration_ReturnsFalse()
        {
            //arrange
            var item = ItemTestData.CreateDefenseEquipmentItem(1);
            var sut = new Decayable(() => item.Metadata, 60);

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
            var item = ItemTestData.CreateDefenseEquipmentItem(1);
            var sut = new Decayable(() => item.Metadata, 2);

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
            var item = ItemTestData.CreateDefenseEquipmentItem(1);
            var sut = new Decayable(() => item.Metadata, 2);

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
            var item = ItemTestData.CreateDefenseEquipmentItem(1);
            var sut = new Decayable(() => item.Metadata, 2);

            //assert
            sut.StartedToDecay.Should().BeFalse();
        }
        [Fact]
        public void Start_IncreaseElapsedTime()
        {
            //arrange
            var item = ItemTestData.CreateDefenseEquipmentItem(1);
            var sut = new Decayable(() => item.Metadata, 20);

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
            var item = ItemTestData.CreateDefenseEquipmentItem(1);
            var sut = new Decayable(() => null, 20);

            //assert
            sut.ShouldDisappear.Should().BeTrue();
        }
        [Fact]
        public void ShouldDisappear_DecaysTo10_ReturnsFalse()
        {
            //arrange
            var item = ItemTestData.CreateDefenseEquipmentItem(1);

            var decayToItem = ItemTestData.CreateDefenseEquipmentItem(10);

            var sut = new Decayable(() => decayToItem.Metadata, 20);

            //assert
            sut.ShouldDisappear.Should().BeFalse();
        }

        [Fact]
        public void Elapsed_2Secs_Returns2()
        {
            //arrange
            var item = ItemTestData.CreateDefenseEquipmentItem(1);
            var decayToItem = ItemTestData.CreateDefenseEquipmentItem(10);

            var sut = new Decayable(() => decayToItem.Metadata, 20);

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
            var item = ItemTestData.CreateDefenseEquipmentItem(1);
            var decayToItem = ItemTestData.CreateDefenseEquipmentItem(10);
            var sut = new Decayable(() => decayToItem.Metadata, 20);

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
            var item = ItemTestData.CreateDefenseEquipmentItem(1);
            var decayToItem = ItemTestData.CreateDefenseEquipmentItem(10);

            var sut = new Decayable(() => decayToItem.Metadata, 20);

            //assert
            sut.Remaining.Should().Be(20);
        }
        [Fact]
        public void Remaining_Elapsed2SecsOf20_Returns18()
        {
            //arrange
            var item = ItemTestData.CreateDefenseEquipmentItem(1);
            var decayToItem = ItemTestData.CreateDefenseEquipmentItem(10);

            var sut = new Decayable(() => decayToItem.Metadata, 20);

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
            var item = ItemTestData.CreateDefenseEquipmentItem(1);
            var decayToItem = ItemTestData.CreateDefenseEquipmentItem(10);

            var sut = new Decayable(() => decayToItem.Metadata, 2);

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
            var item = ItemTestData.CreateDefenseEquipmentItem(1);
            var decayToItem = ItemTestData.CreateDefenseEquipmentItem(10);

            var sut = new Decayable(() => decayToItem.Metadata, 2);

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
            var item = ItemTestData.CreateDefenseEquipmentItem(1);
            var decayToItem = ItemTestData.CreateDefenseEquipmentItem(10);

            var sut = new Decayable(() => decayToItem.Metadata, 2);

            var emitted = false;
            sut.OnStarted += _ => emitted = true;
            //act
            sut.StartDecay();
            //assert
            emitted.Should().BeTrue();
        }

        [Fact]
        public void Elapsed_AfterStarted_Changes()
        {
            //arrange
            var item = ItemTestData.CreateDefenseEquipmentItem(1);
            var decayToItem = ItemTestData.CreateDefenseEquipmentItem(10);

            var sut = new Decayable(() => decayToItem.Metadata, 20);

            //act
            sut.StartDecay();
            Thread.Sleep(1000);
            //assert
            sut.Elapsed.Should().Be(1);
            Thread.Sleep(1000);
            sut.Elapsed.Should().Be(2);
        }
        [Fact]
        public void Remaining_AfterStarted_Changes()
        {
            //arrange
            var item = ItemTestData.CreateDefenseEquipmentItem(1);
            var decayToItem = ItemTestData.CreateDefenseEquipmentItem(10);

            var sut = new Decayable(() => decayToItem.Metadata, 20);

            //act
            sut.StartDecay();
            Thread.Sleep(1000);
            //assert
            sut.Remaining.Should().Be(19);
            Thread.Sleep(1000);
            sut.Remaining.Should().Be(18);
        }

        [Fact]
        public void ToString_DidNotStart_ReturnBrandNewMessage()
        {
            //arrange
            var item = ItemTestData.CreateDefenseEquipmentItem(1);
            var decayToItem = ItemTestData.CreateDefenseEquipmentItem(10);

            var sut = new Decayable(() => decayToItem.Metadata, 20);

            //assert
            sut.ToString().Should().Be("is brand-new");
        }
        [Fact]
        public void ToString_DidStart_ReturnTimeRemaining()
        {
            //arrange
            var item = ItemTestData.CreateDefenseEquipmentItem(1);
            var decayToItem = ItemTestData.CreateDefenseEquipmentItem(10);

            var sut = new Decayable(() => decayToItem.Metadata, 180);

            //act
            sut.StartDecay();
            Thread.Sleep(2000);
            //assert
            sut.ToString().Should().Be("will expire in 2 minutes and 58 seconds");
        }
        [Fact]
        public void ToString_DidStart_ReturnTimeRemainingWellFormatted()
        {
            //arrange
            var item = ItemTestData.CreateDefenseEquipmentItem(1);
            var decayToItem = ItemTestData.CreateDefenseEquipmentItem(10);

            var sut = new Decayable(() => decayToItem.Metadata, 62);

            //act
            sut.StartDecay();
            Thread.Sleep(1000);
            //assert
            sut.ToString().Should().Be("will expire in 1 minute and 1 second");
        }
        [Fact]
        public void ToString_ShowDurationFalse_ReturnEmpty()
        {
            //arrange
            var item = ItemTestData.CreateDefenseEquipmentItem(1);
            var decayToItem = ItemTestData.CreateDefenseEquipmentItem(10);

            var sut = new Decayable(() => decayToItem.Metadata, 62, false);
            //act
            sut.StartDecay();
            //assert
            sut.ToString().Should().BeEmpty();
        }
        [Fact]
        public void Decay_DidNotExpire_ReturnsFalse()
        {
            //arrange
            var item = ItemTestData.CreateDefenseEquipmentItem(1);
            var decayToItem = ItemTestData.CreateDefenseEquipmentItem(10);

            var sut = new Decayable(() => decayToItem.Metadata, 62);

            //act
            sut.StartDecay();
            Thread.Sleep(1000);
            var actual = sut.TryDecay();
            //assert
            actual.Should().BeFalse();
        }

        [Fact]
        public void Decay_DidNotExpire_DoNotEmitEvent()
        {
            //arrange
            var item = ItemTestData.CreateDefenseEquipmentItem(1);
            var decayToItem = ItemTestData.CreateDefenseEquipmentItem(10);

            var sut = new Decayable(() => decayToItem.Metadata, 62);
            var emitted = false;
            sut.OnDecayed += (_) => emitted = true;

            //act
            sut.StartDecay();
            Thread.Sleep(1000);
            sut.TryDecay();
            //assert
            emitted.Should().BeFalse();
        }

        [Fact]
        public void Decay_Expired_ReturnsTrue()
        {
            //arrange
            var item = ItemTestData.CreateDefenseEquipmentItem(1);
            var decayToItem = ItemTestData.CreateDefenseEquipmentItem(10);

            var sut = new Decayable(() => decayToItem.Metadata, duration: 1);

            //act
            sut.StartDecay();
            Thread.Sleep(1500);
            var actual = sut.TryDecay();
            //assert
            actual.Should().BeTrue();
        }
        [Fact]
        public void Decay_Expired_EmitEvent()
        {
            //arrange
            var item = ItemTestData.CreateDefenseEquipmentItem(1);
            var decayToItem = ItemTestData.CreateDefenseEquipmentItem(10);

            var sut = new Decayable(() => decayToItem.Metadata, duration: 1);

            IItemType toItem = null;
            sut.OnDecayed += (to) =>
            {
                toItem = to;
            };

            //act
            sut.StartDecay();
            Thread.Sleep(1500);
            var actual = sut.TryDecay();
            //assert
            toItem.Should().Be(decayToItem.Metadata);
        }
        [Fact]
        public void Decay_ExpiredAndDecaysToNull_EmitEvent()
        {
            //arrange
            var item = ItemTestData.CreateDefenseEquipmentItem(1);

            var sut = new Decayable(() => null, duration: 1);

            IItemType toItem = null;
            sut.OnDecayed += (to) =>
            {
                toItem = to;
            };

            //act
            sut.StartDecay();
            Thread.Sleep(1500);
            var actual = sut.TryDecay();
            //assert
            toItem.Should().BeNull();
        }

        [Fact]
        public void SetDuration_Duration0_Sets()
        {
            //arrange
            var item = ItemTestData.CreateDefenseEquipmentItem(1);

            var sut = new Decayable(() => null, duration: 0);
            //act
            sut.SetDuration(100);
            //assert
            sut.Duration.Should().Be(100);
        }
        [Fact]
        public void SetDuration_Duration100_DoNotSet()
        {
            //arrange
            var item = ItemTestData.CreateDefenseEquipmentItem(1);

            var sut = new Decayable(() => null, duration: 100);
            //act
            sut.SetDuration(200);
            //assert
            sut.Duration.Should().Be(100);
        }

    }
}
