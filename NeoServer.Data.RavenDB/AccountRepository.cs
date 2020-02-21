using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeoServer.Server.Contracts.Repositories;
using NeoServer.Server.Model;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session;

namespace NeoServer.Data.RavenDB
{

    public class AccountRepository : AbstractIndexCreationTask<Account>, IAccountRepository
    {
        public async void Create(Account account)
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
        public async Task<Account> Get(string account, string password)
        {

            using (IAsyncDocumentSession Session = Database.OpenAsyncSession())
            {
                return await Session.Query<Account>()
            .FirstOrDefaultAsync(a => a.AccountName == account &&
                a.Password == password);
            }

        }

    }
}