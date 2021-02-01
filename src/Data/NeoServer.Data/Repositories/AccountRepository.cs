using Dapper;
using Microsoft.EntityFrameworkCore;
using NeoServer.Data.Interfaces;
using NeoServer.Data.Model;
using NeoServer.Server.Model.Players;
using System.Collections.Generic;
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

        /// <summary>
        /// This method is responsible for get account by id in database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Account</returns>
        public async Task<AccountModel> GetById(int id)
        {
            return await Context.Accounts
                .Where(c => c.AccountId.Equals(id))

                .SingleOrDefaultAsync();
        }

        /// <summary>
        /// This method is responsible for get account by name in database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Account</returns>
        public async Task<AccountModel> GetByName(string name)
        {
            return await Context.Accounts
                .Where(x => x.Name.Equals(name))
                .Include(x => x.Players)

                .SingleOrDefaultAsync();
        }

        public async Task<AccountModel> GetByEmail(string email)
        {
            return await Context.Accounts
                    .Where(x => x.Email.Equals(email))
                    .Include(x => x.Players)
                .SingleOrDefaultAsync();
        }

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

        public IQueryable<PlayerModel> GetPlayer(string accountName, string password, string charName)
        {
            return Context.Players.Where(x => x.Account.Name.Equals(accountName, System.StringComparison.InvariantCultureIgnoreCase) &&
                                         x.Account.Password.Equals(password) &&
                                         x.Name.Equals(charName, System.StringComparison.InvariantCultureIgnoreCase));
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
