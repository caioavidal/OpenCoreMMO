using Microsoft.EntityFrameworkCore;
using NeoServer.Data.Interfaces;
using NeoServer.Data.Model;
using System.Linq;
using System.Threading.Tasks;

namespace NeoServer.Data.Repositories
{
    public class AccountRepositoryNeo : BaseRepositoryNeo<AccountModel, NeoContext>, IAccountRepositoryNeo
    {
        #region constructors

        public AccountRepositoryNeo(NeoContext context) : base(context) { }

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
                //.Where(c => c.AccountId.Equals(id))
                //.Include(c => c.Players)
                //.Include(x => x.AccountIdentity)
                //.Include(c => c.AccountBan)
                //    .ThenInclude(c => c.Account)
                //.Include(c => c.AccountBan)
                //    .ThenInclude(c => c.BannedBy)
                //.Include(c => c.AccountBanHistory)
                //    .ThenInclude(c => c.Account)
                //.Include(c => c.AccountBanHistory)
                //    .ThenInclude(c => c.BannedBy)
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
                //.Where(x => x.Name == name)
                .Include(x => x.Players)
                //    .Include(x => x.AccountIdentity)
                //    .Include(x => x.AccountBan)
                //        .ThenInclude(x => x.Account)
                //    .Include(x => x.AccountBan)
                //        .ThenInclude(x => x.BannedBy)
                //    .Include(x => x.AccountBanHistory)
                //        .ThenInclude(x => x.Account)
                //    .Include(x => x.AccountBanHistory)
                //        .ThenInclude(x => x.BannedBy)
                .SingleOrDefaultAsync();
        }

        public async Task<AccountModel> GetByEmail(string email)
        {
            return await Context.Accounts
                    .Where(x => x.Email.Equals(email))
                    //.Where(x => x.Email == email)
                    .Include(x => x.Players)
                    //.Include(x => x.AccountIdentity)
                    //.Include(x => x.AccountBan)
                    //    .ThenInclude(x => x.Account)
                    //.Include(x => x.AccountBan)
                    //    .ThenInclude(x => x.BannedBy)
                    //.Include(x => x.AccountBanHistory)
                    //    .ThenInclude(x => x.Account)
                    //.Include(x => x.AccountBanHistory)
                    //    .ThenInclude(x => x.BannedBy)
                .SingleOrDefaultAsync();
        }

        public async Task<AccountModel> Login(string name, string password)
        {
            return await Context.Accounts
                    .Include(x=>x.Players)
                    .ThenInclude(x=>x.PlayerItems)
                    .Where(x => x.Name.Equals(name) && x.Password.Equals(password))
                    //.Where(x => x.Email == email)
                    .Include(x => x.Players)
                        .ThenInclude(x => x.PlayerInventoryItems)
                    //.Include(x => x.AccountIdentity)
                    //.Include(x => x.AccountBan)
                    //    .ThenInclude(x => x.Account)
                    //.Include(x => x.AccountBan)
                    //    .ThenInclude(x => x.BannedBy)
                    //.Include(x => x.AccountBanHistory)
                    //    .ThenInclude(x => x.Account)
                    //.Include(x => x.AccountBanHistory)
                    //    .ThenInclude(x => x.BannedBy)
                    .SingleOrDefaultAsync();
        }

        #endregion
    }
}
