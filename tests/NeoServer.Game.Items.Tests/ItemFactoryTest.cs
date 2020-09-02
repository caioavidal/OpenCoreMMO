using NeoServer.Game.Enums.Location.Structs;
using Xunit;

namespace NeoServer.Game.Items.Tests
{
    public class ItemFactoryTest
    {

        [Fact]
        public void Create_When_TypeId_Less_Than_100_Returns_Null()
        {
            var sup = ItemFactory.Create(99, new Location(), null);

            Assert.Null(sup);
        }

    }
}
