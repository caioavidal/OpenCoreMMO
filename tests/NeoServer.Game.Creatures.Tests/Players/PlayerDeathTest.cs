using System;
using System.Collections.Generic;
using FluentAssertions;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures.Model.Players;
using NeoServer.Game.Tests.Helpers;
using NeoServer.Game.World.Models;
using Xunit;

namespace NeoServer.Game.Creatures.Tests.Players;

public class PlayerDeathTest
{
    [Fact]
    public void Player_lost_experience_on_death()
    {
        var player = PlayerTestDataBuilder.Build(hp: 100, skills: new Dictionary<SkillType, ISkill>
        {
            { SkillType.Level, new Skill(SkillType.Level, level: 9, count: 9100) }
        });
        (player as Player).OnDeath(null);

        Assert.Equal(8190, (double)player.Experience);
        Assert.Equal(9, player.Level);
    }

    [Fact]
    public void Player_lost_level_on_death()
    {
        var player = PlayerTestDataBuilder.Build(hp: 100, skills: new Dictionary<SkillType, ISkill>
        {
            { SkillType.Level, new Skill(SkillType.Level, level: 9, count: 6500) }
        });
        (player as Player).OnDeath(null);

        Assert.Equal(5850, (double)player.Experience);
        Assert.Equal(8, player.Level);
    }


    [Fact]
    public void Player_has_changed_local_to_temple_on_death()
    {
        var townCoordinate = new Coordinate(1000, 2033, 8);
        
        var player = PlayerTestDataBuilder.Build(hp: 100,  town: new Town() { Coordinate = townCoordinate });
        var p = (player as Player);

        p.Location = new Location(1234,1341, 3);

        Assert.NotEqual(p.Location, townCoordinate.Location);
        
        p.OnDeath(null);
        
        Assert.Equal(p.Location, townCoordinate.Location);
    }
}
