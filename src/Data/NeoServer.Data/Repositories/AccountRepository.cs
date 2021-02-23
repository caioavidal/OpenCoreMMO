using Microsoft.EntityFrameworkCore;
using NeoServer.Data.Interfaces;
using NeoServer.Data.Model;
using NeoServer.Server.Model.Players;
using System.Linq;
using System.Threading.Tasks;

namespace NeoServer.Data.Repositories
{
    public class AccountRepository : BaseRepository<AccountModel, NeoContext>, IAccountRepository
    {
        #region constructors

        public AccountRepository(NeoContext context) : base(context) { }

        #endregion

        #region public methods implementation

        public IQueryable<AccountModel> GetAccount(string name, string password)
        {
            return Context.Accounts.Where(x => x.Name.Equals(name) && x.Password.Equals(password)).AsQueryable();
            //.Include(x => x.Players)
            //.ThenInclude(x => x.PlayerItems)

            //.Include(x => x.Players)
            //.ThenInclude(x => x.PlayerInventoryItems)
            //.Include(x => x.VipList)
            //.ThenInclude(x => x.Player)
            //.SingleOrDefaultAsync();
        }

        public async Task<PlayerModel> GetPlayer(string accountName, string password, string charName)
        {
            return await Context.Players.Where(x => x.Account.Name.Equals(accountName) &&
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

        public async Task<PlayerModel> GetPlayer(string playerName)
        {
            return await Context.Players.FirstOrDefaultAsync(x => x.Name.Equals(playerName));
        }
        public async Task AddPlayerToVipList(int accountId, int playerId)
        {
            await Context.AccountsVipList.AddAsync(new AccountVipListModel()
            {
                AccountId = accountId,
                PlayerId = playerId,
            });

            await CommitChanges();
        }
        public async Task RemoveFromVipList(int accountId, int playerId)
        {
            var item = await Context.AccountsVipList.SingleOrDefaultAsync(x => x.PlayerId == playerId && x.AccountId == accountId);
            Context.AccountsVipList.Remove(item);
            await CommitChanges();
        }


        #endregion
    }
}
