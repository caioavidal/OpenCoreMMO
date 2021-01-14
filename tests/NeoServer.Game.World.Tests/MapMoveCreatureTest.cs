using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Tests;
using NeoServer.Game.Tests.Helpers;
using Xunit;

namespace NeoServer.Game.World.Tests
{
    public class MapMoveCreatureTest
    {
        [Fact]
        public void TryMoveCreature_Should_Move_Creature()
        {
            var sup = MapTestDataBuilder.CreateMap(1, 101, 1, 101, 6, 9);
            var player = PlayerTestDataBuilder.BuildPlayer();
            player.SetNewLocation(new Location(50, 50, 7));
            var result = sup.TryMoveCreature(player, new Location(51, 50, 7));

            Assert.True(result);
            Assert.Equal(new Location(51, 50, 7), player.Location);
        }
        [Fact]
        public void TryMoveCreature_when_Teleport_Should_Move_Creature()
        {
            var sup = MapTestDataBuilder.CreateMap(1, 101, 1, 101, 6, 9);
            var player = PlayerTestDataBuilder.BuildPlayer();
            player.SetNewLocation(new Location(50, 50, 7));
            var result = sup.TryMoveCreature(player, new Location(53, 50, 7));

            Assert.True(result);
            Assert.Equal(new Location(53, 50, 7), player.Location);
        }
    }
}
