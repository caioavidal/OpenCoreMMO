using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NeoServer.Data.Contexts;
using NeoServer.Data.Interfaces;
using NeoServer.Data.Model;
using Serilog;

namespace NeoServer.Data.Repositories;

public class AccountRepository : BaseRepository<AccountModel>, IAccountRepository
{
    #region constructors

    public AccountRepository(DbContextOptions<NeoContext> contextOptions, ILogger logger) : base(contextOptions,
        logger)
    {
    }

    #endregion

    #region public methods implementation

    #region gets

    public async Task<AccountModel> GetAccount(string name, string password)
    {
        await using var context = NewDbContext;

        return await context.Accounts
            .Where(x => x.Name.Equals(name) && x.Password.Equals(password))
            .Include(x => x.Players)
            .ThenInclude(x => x.World)
            .SingleOrDefaultAsync();
    }

    public async Task<PlayerModel> GetPlayer(string accountName, string password, string charName)
    {
        await using var context = NewDbContext;

        return await context.Players.Where(x => x.Account.Name.Equals(accountName) &&
                                                x.Account.Password.Equals(password) &&
                                                x.Name.Equals(charName))
            .Include(x => x.PlayerItems)
            .Include(x => x.PlayerInventoryItems)
            .Include(x => x.Account)
            .ThenInclude(x => x.VipList)
            .ThenInclude(x => x.Player)
            .Include(x => x.GuildMember)
            .ThenInclude(x => x.Guild).SingleOrDefaultAsync();
    }
    
    public async Task<PlayerModel> GetOnlinePlayer(string accountName)
    {
        await using var context = NewDbContext;

        return await context.Players
            .Include(x => x.Account)
            .Where(x => x.Account.Name.Equals(accountName) && x.Online)
            .FirstOrDefaultAsync();
    }


    #endregion

    #region inserts

    public async Task AddPlayerToVipList(int accountId, int playerId)
    {
        await using var context = NewDbContext;

        await context.AccountsVipList.AddAsync(new AccountVipListModel
        {
            AccountId = accountId,
            PlayerId = playerId
        });

        await CommitChanges(context);
    }

    #endregion

    #region updates

    public async Task<int> Ban(uint accountId, string reason, uint bannedByAccountId)
    {
        await using var context = NewDbContext;

        return await context.Accounts
            .Where(x => x.AccountId == accountId)
            .ExecuteUpdateAsync(x
                => x.SetProperty(y => y.BannedBy, bannedByAccountId)
                    .SetProperty(y => y.BanishmentReason, reason)
                    .SetProperty(y => y.BanishedAt, DateTime.Now));
    }

    #endregion
    
    #region deletes
    public async Task RemoveFromVipList(int accountId, int playerId)
    {
        await using var context = NewDbContext;

        var item = await context.AccountsVipList.SingleOrDefaultAsync(x =>
            x.PlayerId == playerId && x.AccountId == accountId);

        if (item is null) return;

        context.AccountsVipList.Remove(item);
        await CommitChanges(context);
    }
    #endregion
    
    #endregion
}