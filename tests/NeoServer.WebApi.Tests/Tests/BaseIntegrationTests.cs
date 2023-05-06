using NeoServer.WebApi.Tests.Factories;
using Microsoft.Extensions.DependencyInjection;
using NeoServer.Data.Contexts;
using NeoServer.Data.Model;
using NeoServer.Game.World;

namespace NeoServer.WebApi.Tests.Tests
{
    public class BaseIntegrationTests
    {
        #region Protected Members

        protected readonly NeoContext _neoContext;
        protected readonly NeoServerWebApiWebApplicationFactory _neoFactory;
        protected readonly HttpClient _neoHttpClient;
        protected static object _lock = new object();

        #endregion

        #region Constructors

        public BaseIntegrationTests()
        {
            lock (_lock)
            {
                _neoFactory = NeoServerWebApiWebApplicationFactory.GetInstance();
                _neoHttpClient = _neoFactory.CreateClient();
                var serviceScope = _neoFactory.Services.GetService<IServiceScopeFactory>().CreateScope();
                _neoContext = serviceScope?.ServiceProvider.GetService<NeoContext>();
            }
        }

        #endregion

        #region Protected Methods

        protected string GenerateRandomString(int length, bool onlyNumbers = false, bool onlyLetters = false)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            if (onlyNumbers)
                chars = "0123456789";
            else if (onlyLetters)
                chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        protected async Task<AccountModel> CreateAccount()
        {
            var lastAccount = _neoContext.Accounts.OrderBy(c => c.AccountId).LastOrDefault();

            var lastId = 0;

            if (lastAccount != null)
                lastId = lastAccount.AccountId;

            var account = new AccountModel
            {
                AccountId = ++lastId,
                Name = GenerateRandomString(10),
                Email = GenerateRandomString(10),
                Password = GenerateRandomString(10),
            };

            await _neoContext.Accounts.AddAsync(account);
            await _neoContext.SaveChangesAsync();

            return account;
        }

        protected async Task<WorldModel> CreateWorld()
        {
            var lastWorld = _neoContext.Worlds.OrderBy(c => c.Id).LastOrDefault();

            var lastId = 0;

            if (lastWorld != null)
                lastId = lastWorld.Id;

            var world = new WorldModel
            {
                Id = ++lastId,
                Name = GenerateRandomString(10),
                Ip = GenerateRandomString(10),
            };

            await _neoContext.Worlds.AddAsync(world);
            await _neoContext.SaveChangesAsync();

            return world;
        }


        protected async Task<PlayerModel> CreatePlayer()
        {
            var lastAccount = _neoContext.Accounts.OrderBy(c => c.AccountId).LastOrDefault();

            if (lastAccount == null)
            {
                lastAccount = await CreateAccount();
            }

            var lastWorld = _neoContext.Worlds.OrderBy(c => c.Id).LastOrDefault();

            if (lastWorld == null)
            {
                lastWorld = await CreateWorld();
            }

            var lastPlayer = _neoContext.Players.OrderBy(c => c.PlayerId).LastOrDefault();

            var lastId = 0;

            if (lastPlayer != null)
                lastId = lastPlayer.PlayerId;

            var player = new PlayerModel
            {
                AccountId = lastAccount.AccountId,
                WorldId = lastWorld.Id,
                PlayerId = ++lastId,
                Name = GenerateRandomString(10),
                Level = 1,
            };

            await _neoContext.Players.AddAsync(player);
            await _neoContext.SaveChangesAsync();

            return player;
        }

        #endregion
    }
}