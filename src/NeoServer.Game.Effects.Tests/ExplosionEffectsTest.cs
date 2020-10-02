using NeoServer.Game.Effects.Explosion;
using Xunit;

namespace NeoServer.Game.Effects.Tests
{
    public class ExplosionEffectsTest
    {
        [Theory]
        [InlineData(1)]
        public void Create_Should_Create_Matrix_Of_Locations(int radius)
        {

            var a = ExplosionEffect.Create(radius);
        }
    }
}