using System.Collections.Generic;
using Moq;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Items.Protections;
using NeoServer.Game.Tests.Helpers;
using Xunit;

namespace NeoServer.Game.Items.Tests.Items.Protections
{
    public class NecklaceTest
    {
        [Fact]
        public void DressedIn_When_Has_Damage_Protection_Should_Reduce_Damage()
        {
            var itemType = new Mock<IItemType>();

            itemType.SetupGet(x => x.Attributes).Returns(new ItemAttributeList());

            itemType.SetupGet(x => x.Attributes.DamageProtection).Returns(new Dictionary<DamageType, byte>
            {
                {DamageType.Fire, 20}
            });

            var sut = new Necklace(itemType.Object, Location.Zero, 5, 0);

            var player = PlayerTestDataBuilder.BuildPlayer();
            var enemy = PlayerTestDataBuilder.BuildPlayer();

            sut.DressedIn(player);

            var resultDamage = 0;
            player.OnDamaged += (_, _, damage) =>
            {
                resultDamage = damage.Damage;
            };

            var damage = new CombatDamage(200, DamageType.Fire);
            player.ReceiveAttack(enemy, damage);

            Assert.Equal(160, resultDamage);
        }
        [Fact]
        public void DressedIn_When_Has_No_Damage_Protection_Should_Not_Reduce_Damage()
        {
            var itemType = new Mock<IItemType>();

            itemType.SetupGet(x => x.Attributes).Returns(new ItemAttributeList());

            itemType.SetupGet(x => x.Attributes.DamageProtection).Returns(new Dictionary<DamageType, byte>
            {
                {DamageType.Earth, 20}
            });

            var sut = new Necklace(itemType.Object, Location.Zero, 5, 0);

            var player = PlayerTestDataBuilder.BuildPlayer();
            var enemy = PlayerTestDataBuilder.BuildPlayer();

            sut.DressedIn(player);

            var resultDamage = 0;
            player.OnDamaged += (_, _, damage) =>
            {
                resultDamage = damage.Damage;
            };

            var damage = new CombatDamage(200, DamageType.Fire);
            player.ReceiveAttack(enemy, damage);

            Assert.Equal(200, resultDamage);
        }
        [Fact]
        public void DressedIn_When_Has_100_Percent_Damage_Protection_Should_Reduce_Damage_To_0()
        {
            var itemType = new Mock<IItemType>();

            itemType.SetupGet(x => x.Attributes).Returns(new ItemAttributeList());

            itemType.SetupGet(x => x.Attributes.DamageProtection).Returns(new Dictionary<DamageType, byte>
            {
                {DamageType.Fire, 100}
            });

            var sut = new Necklace(itemType.Object, Location.Zero, 5, 0);

            var player = PlayerTestDataBuilder.BuildPlayer();
            var enemy = PlayerTestDataBuilder.BuildPlayer();

            sut.DressedIn(player);

            var resultDamage = 0;
            player.OnDamaged += (_, _, damage) =>
            {
                resultDamage = damage.Damage;
            };

            var damage = new CombatDamage(200, DamageType.Fire);
            player.ReceiveAttack(enemy, damage);

            Assert.Equal(0, resultDamage);
        }
        [Fact]
        public void UndressFrom_When_Receive_Damage_Should_Not_Reduce_Damage()
        {
            var itemType = new Mock<IItemType>();

            itemType.SetupGet(x => x.Attributes).Returns(new ItemAttributeList());

            itemType.SetupGet(x => x.Attributes.DamageProtection).Returns(new Dictionary<DamageType, byte>
            {
                {DamageType.Fire, 100}
            });

            var sut = new Necklace(itemType.Object, Location.Zero, 5, 0);

            var player = PlayerTestDataBuilder.BuildPlayer();
            var enemy = PlayerTestDataBuilder.BuildPlayer();

            sut.DressedIn(player);

            var resultDamage = 0;
            player.OnDamaged += (_, _, damage) =>
            {
                resultDamage = damage.Damage;
            };

            var damage = new CombatDamage(200, DamageType.Fire);
            player.ReceiveAttack(enemy, damage);

            Assert.Equal(0, resultDamage);

            sut.UndressFrom(player);

            player.ReceiveAttack(enemy, damage);

            Assert.Equal(200, resultDamage);
        }
    }
}
