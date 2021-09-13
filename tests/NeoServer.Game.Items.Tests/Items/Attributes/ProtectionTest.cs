using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Items;
using NeoServer.Game.Tests.Helpers;
using Xunit;

namespace NeoServer.Game.Items.Tests.Items.Protections
{
    public class ProtectionTest
    {
        [Fact]
        public void DressedIn_When_Has_Damage_Protection_Should_Reduce_Damage()
        {
            var sut = ItemTestData.CreateDefenseEquipmentItem(1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.AbsorbPercentFire, 20)
            }, charges: 10);

            var player = PlayerTestDataBuilder.BuildPlayer();
            var enemy = PlayerTestDataBuilder.BuildPlayer();

            sut.DressedIn(player);

            var resultDamage = 0;
            player.OnInjured += (_, _, damage) => { resultDamage = damage.Damage; };

            var damage = new CombatDamage(200, DamageType.Fire);
            player.ReceiveAttack(enemy, damage);

            Assert.Equal(160, resultDamage);
        }

        [Fact]
        public void DressedIn_When_Has_No_Damage_Protection_Should_Not_Reduce_Damage()
        {
            var itemType = new Mock<IItemType>();

            itemType.SetupGet(x => x.Attributes).Returns(new ItemAttributeList());

            itemType.SetupGet(x => x.Attributes.DamageProtection).Returns(new Dictionary<DamageType, sbyte>
            {
                {DamageType.Earth, 20}
            });

            var sut = new BodyDefenseEquipment(itemType.Object, Location.Zero);

            var player = PlayerTestDataBuilder.BuildPlayer();
            var enemy = PlayerTestDataBuilder.BuildPlayer();

            sut.DressedIn(player);

            var resultDamage = 0;
            player.OnInjured += (_, _, damage) => { resultDamage = damage.Damage; };

            var damage = new CombatDamage(200, DamageType.Fire);
            player.ReceiveAttack(enemy, damage);

            Assert.Equal(200, resultDamage);
        }

        [Fact]
        public void DressedIn_When_Has_100_Percent_Damage_Protection_Should_Reduce_Damage_To_0()
        {
            var sut = ItemTestData.CreateDefenseEquipmentItem(1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.AbsorbPercentFire, 100)
            }, charges: 10 );

            var player = PlayerTestDataBuilder.BuildPlayer();
            var enemy = PlayerTestDataBuilder.BuildPlayer();

            sut.DressedIn(player);

            var resultDamage = 0;
            player.OnInjured += (_, _, damage) => { resultDamage = damage.Damage; };

            var damage = new CombatDamage(200, DamageType.Fire);
            player.ReceiveAttack(enemy, damage);

            Assert.Equal(0, resultDamage);
        }


        [Fact]
        public void DressedIn_100PercentDamageProtection_From_Different_Attack_Should_Not_Reduce_Damage()
        {
            var sut = ItemTestData.CreateDefenseEquipmentItem(1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.AbsorbPercentFire, 100)
            }, charges: 10);

            var player = PlayerTestDataBuilder.BuildPlayer();
            var enemy = PlayerTestDataBuilder.BuildPlayer();

            sut.DressedIn(player);

            var resultDamage = 0;
            player.OnInjured += (_, _, damage) => { resultDamage = damage.Damage; };

            var damage = new CombatDamage(200, DamageType.Energy);
            player.ReceiveAttack(enemy, damage);

            resultDamage.Should().BeGreaterThan(0);
        }

        [Fact]
        public void UndressFrom_When_Receive_Damage_Should_Not_Reduce_Damage()
        {
            var sut = ItemTestData.CreateDefenseEquipmentItem(1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.AbsorbPercentFire, 100)
            }, charges: 10);

            var player = PlayerTestDataBuilder.BuildPlayer();
            var enemy = PlayerTestDataBuilder.BuildPlayer();

            sut.DressedIn(player);

            var resultDamage = 0;
            player.OnInjured += (_, _, damage) => { resultDamage = damage.Damage; };

            var damage = new CombatDamage(200, DamageType.Fire);
            player.ReceiveAttack(enemy, damage);

            Assert.Equal(0, resultDamage);

            sut.UndressFrom(player);

            player.ReceiveAttack(enemy, damage);

            Assert.Equal(200, resultDamage);
        }
        [Fact]
        public void Decrease_DefendedAttack_DecreaseCharges()
        {
            //arrange
            var defender = PlayerTestDataBuilder.BuildPlayer();
            var attacker = PlayerTestDataBuilder.BuildPlayer();

            var hmm = ItemTestData.CreateAttackRune(1, damageType: DamageType.Energy);

            var sut = ItemTestData.CreateDefenseEquipmentItem(1, charges: 50, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.AbsorbPercentEnergy,10)
            });

            sut.DressedIn(defender);

            //act
            attacker.Attack(defender, hmm);

            //assert
            sut.Charges.Should().Be(49);
        }

        [Fact]
        public void Decrease_DefendedDifferentAttack_DoNotDecreaseCharges()
        {
            //arrange
            var defender = PlayerTestDataBuilder.BuildPlayer();
            var attacker = PlayerTestDataBuilder.BuildPlayer();

            var hmm = ItemTestData.CreateAttackRune(1, damageType: DamageType.Energy);

            var sut = ItemTestData.CreateDefenseEquipmentItem(1, charges: 50, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.AbsorbPercentFire,100)
            });

            sut.DressedIn(defender);

            //act
            attacker.Attack(defender, hmm);

            //assert
            sut.Charges.Should().Be(50);
        }
        [Fact]
        public void Protect_InfiniteCharges_Protect()
        {
            //arrange
            var defender = PlayerTestDataBuilder.BuildPlayer();
            var attacker = PlayerTestDataBuilder.BuildPlayer();
            var oldHp = defender.HealthPoints;

            var totalDamage = 0;
            defender.OnInjured += (enemy, victim, damage) =>
            {
                totalDamage = damage.Damage;
            };

            var hmm = ItemTestData.CreateAttackRune(1, damageType: DamageType.Energy, min: 100, max: 100);

            var sut = ItemTestData.CreateDefenseEquipmentItem(1, charges: 0,
                attributes: new (ItemAttribute, IConvertible)[]
                {
                    (ItemAttribute.AbsorbPercentEnergy, 100),
                    (ItemAttribute.Duration, 100)
                });
            sut.DressedIn(defender);

            //act
            attacker.Attack(defender, hmm);

            //assert
            totalDamage.Should().Be(0);
            defender.HealthPoints.Should().Be(oldHp);
        }

        [Fact]
        public void Protect_NoCharges_DoNotProtect()
        {
            //arrange
            var defender = PlayerTestDataBuilder.BuildPlayer();
            var attacker = PlayerTestDataBuilder.BuildPlayer();
            var oldHp = defender.HealthPoints;

            var totalDamage = 0;
            defender.OnInjured += (enemy, victim, damage) =>
            {
                totalDamage = damage.Damage;
            };

            var hmm = ItemTestData.CreateAttackRune(1, damageType: DamageType.Energy, min: 100, max: 100);

            var sut = ItemTestData.CreateDefenseEquipmentItem(1, charges: 1);
            sut.Metadata.Attributes.SetAttribute(ItemAttribute.AbsorbPercentEnergy, 100);
            sut.DressedIn(defender);

            //act
            attacker.Attack(defender, hmm);
            attacker.Attack(defender, hmm);

            //assert
            totalDamage.Should().NotBe(0);
            defender.HealthPoints.Should().BeLessThan(oldHp);
        }
        [Fact]
        public void Protect_1Charge_ProtectFromDamage()
        {
            //arrange
            var defender = PlayerTestDataBuilder.BuildPlayer();
            var attacker = PlayerTestDataBuilder.BuildPlayer();

            var oldHp = defender.HealthPoints;

            var totalDamage = 0;
            defender.OnInjured += (_, _, damage) =>
            {
                totalDamage = damage.Damage;
            };

            var hmm = ItemTestData.CreateAttackRune(1, damageType: DamageType.Energy, min: 100, max: 100);

            var sut = ItemTestData.CreateDefenseEquipmentItem(1, charges: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.AbsorbPercentEnergy, 100),
                (ItemAttribute.Duration, 100)
            });
            sut.DressedIn(defender);

            //act
            attacker.Attack(defender, hmm);

            //assert
            totalDamage.Should().Be(0);
            defender.HealthPoints.Should().Be(oldHp);
        }

        #region Negative protection

        [Theory]
        [InlineData(-100,400, 100 )]
        [InlineData(-50, 300, 200)]
        [InlineData(-5, 210, 290)]
        public void DressedIn_When_Player_Has_Negative_Damage_Protection_Should_Increase_Damage(sbyte protection, ushort expectedDamage, ushort remainingHp)
        {
            //arrange
            var defender = PlayerTestDataBuilder.BuildPlayer(hp: 500);
            var attacker = PlayerTestDataBuilder.BuildPlayer();
            var oldHp = defender.HealthPoints;

            var totalDamage = 0;
            defender.OnInjured += (enemy, victim, damage) =>
            {
                totalDamage = damage.Damage;
            };

            var hmm = ItemTestData.CreateAttackRune(1, damageType: DamageType.Energy, min: 100, max: 100);

            var sut = ItemTestData.CreateDefenseEquipmentItem(1, charges: 0,
                attributes: new (ItemAttribute, IConvertible)[]
                {
                    (ItemAttribute.AbsorbPercentEnergy, protection),
                });
            sut.DressedIn(defender);

            //act
            var damage = new CombatDamage(200, DamageType.Energy);
            defender.ReceiveAttack(attacker, damage);

            //assert
            totalDamage.Should().Be(expectedDamage);
            defender.HealthPoints.Should().Be(remainingHp);
        }

        #endregion
    }
}