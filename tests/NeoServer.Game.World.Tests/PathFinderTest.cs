using NeoServer.Game.World.Map;
using Xunit;

namespace NeoServer.Game.World.Tests
{
    public class PathFinderTest
    {
        [Fact]
        public void Find()
        {
            //var sut = new PathFinder();
            //sut.Find(new Location(100, 100, 7), new Location(105, 109, 7));
            var sut = new AStarTibia();

            //var result = sut.GetPathMatching(null, new Location(100, 100, 7), new Location(103, 100, 7), keepDistance: false);
        }
    }
}
