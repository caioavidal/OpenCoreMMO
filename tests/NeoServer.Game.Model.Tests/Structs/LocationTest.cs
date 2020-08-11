using NeoServer.Game.Enums.Location.Structs;
using Xunit;

namespace NeoServer.Game.Model.Tests.Structs
{
    public class LocationTest
    {
        [Theory]
        [InlineData(100, 100, 105, 103, 8)]
        [InlineData(105, 103, 100, 100, 8)]
        [InlineData(100, 100, -105, -103, 408)]
        [InlineData(100, 100, -105, 103, 208)]
        [InlineData(100, 0, 0, 100, 200)]
        public void GetSqmDistance_Returns_Sum_Of_Sqm_Distance(int fromX, int fromY, int toX, int toY, ushort total)
        {
            var fromLocation = new Location(fromX, fromY, 7);
            var toLocation = new Location(toX, toY, 7);

            Assert.Equal(total, fromLocation.GetSqmDistance(toLocation));
        }
    }
}
