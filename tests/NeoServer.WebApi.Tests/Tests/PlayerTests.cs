using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NeoServer.Web.Shared.ViewModels.Response;
using NeoServer.Web.Shared.Extensions;
using Xunit;

namespace NeoServer.WebApi.Tests.Tests
{
    public class PlayerTests : BaseIntegrationTests
    {
        #region Constructors

        public PlayerTests() { }

        #endregion

        #region Get Tests

        [Fact(DisplayName = "Get All Players")]
        public async Task Get_All_Players()
        {
            // Arrange
            var productsCount = await _neoContext.Players.CountAsync();

            // Act
            var response =
                await _neoHttpClient.GetAndDeserialize<IEnumerable<PlayerResponseViewModel>>("api/Player");

            //Assert
            response.Count().Should().Be(productsCount);
        }

        [Fact(DisplayName = "Get Player By Id")]
        public async Task Get_Player_By_Id()
        {
            // Arrange
            var player = await CreatePlayer();

            // Act
            var response =
                await _neoHttpClient.GetAndDeserialize<PlayerResponseViewModel>($"api/Player/{player.PlayerId}");

            //Assert
            response.Name.Should().Be(player.Name);
        }

        #endregion
    }
}
