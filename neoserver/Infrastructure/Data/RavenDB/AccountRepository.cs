using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session;

public class AccountRepository : AbstractIndexCreationTask<Account>
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
            //.ToListAsync();
        }

    }

}