using NeoServer.Data.Model;
using NeoServer.Server.Contracts.Repositories;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NeoServer.Data.RavenDB
{

    public class AccountRepository : BaseRepository<AccountModel>, IAccountRepository
    {
        public AccountRepository(Database database) : base(database)
        {
        }

        /// <summary>
        /// Creates new account
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public async void Create(AccountModel account)
        {
            using (var store = new DocumentStore())
            {
                // Open a Session in asynchronous operation mode for cluster-wide transactions

                SessionOptions options = new SessionOptions
                {

                    Database = "neoserver",
                    TransactionMode = TransactionMode.ClusterWide
                };
                store.Urls = new[] { "http://localhost:8080" };
                store.Initialize();

                using (IAsyncDocumentSession Session = store.OpenAsyncSession(options))
                {
                    await Session.StoreAsync(account);
                    await Session.SaveChangesAsync();
                }
            }
        }

        /// <summary>
        /// Gets account record
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<AccountModel> FirstOrDefaultAsync(Expression<Func<AccountModel, bool>> filter)
        {

            using (IAsyncDocumentSession Session = Database.OpenAsyncSession())
            {
                return await Session.Query<AccountModel>().FirstOrDefaultAsync(filter);
            }

        }

        /// <summary>
        /// Gets account record
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<AccountModel> Get(string account, string password)
        {

            using (IAsyncDocumentSession Session = Database.OpenAsyncSession())
            {
                return await Session.Query<AccountModel>()
            .FirstOrDefaultAsync(a => a.AccountName == account &&
                a.Password == password);
            }

        }

    }
}