using System.Threading;
using FluentAssertions;
using NeoServer.Game.Tests.Helpers;
using Xunit;

namespace NeoServer.Game.Creatures.Tests.Creature;

public class MonsterTest
{
    [Fact]
    public void Monster_is_not_injured_when_attacked_by_another_monster()
    {
        //arrange
        var sut = MonsterTestDataBuilder.Build();
        var attacker = MonsterTestDataBuilder.Build();
        using var monitor = sut.Monitor();

        //act
        attacker.Attack(sut);

        //assert
        sut.HealthPoints.Should().Be(sut.MaxHealthPoints);
        monitor.Should().NotRaise(nameof(sut.OnAttacked));
        monitor.Should().NotRaise(nameof(sut.OnInjured));
    }
    
    [Fact]
    public void Monster_is_not_injured_when_attacked_by_a_summon_of_a_monster()
    {
        //arrange
        var sut = MonsterTestDataBuilder.Build();
        
        var master =  MonsterTestDataBuilder.Build();
        var summon =  MonsterTestDataBuilder.BuildSummon(master);
        
        using var monitor = sut.Monitor();

        //act
        summon.Attack(sut);

        //assert
        sut.HealthPoints.Should().Be(sut.MaxHealthPoints);
        monitor.Should().NotRaise(nameof(sut.OnAttacked));
        monitor.Should().NotRaise(nameof(sut.OnInjured));
    }
    
    [Fact]
    public void Monster_is_injured_when_attacked_by_a_summon_of_a_player()
    {
        //arrange
        var sut = MonsterTestDataBuilder.Build(9000);
        
        var master =  PlayerTestDataBuilder.Build();
        var summon =  MonsterTestDataBuilder.BuildSummon(master, minDamage:4000, maxDamage: 5000);
        
        //act
        summon.Attack(sut);

        //assert
        sut.HealthPoints.Should().BeLessThan(sut.MaxHealthPoints);
    }
}