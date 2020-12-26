//using NeoServer.Data.Model;
//using NeoServer.Game.Contracts;
//using NeoServer.Server.Contracts.Repositories;
//using Raven.Client.Documents;
//using Raven.Client.Documents.Session;
//using System;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Threading.Tasks;

//namespace NeoServer.Data.RavenDB
//{

//    public class AccountRepository : BaseRepository<IAccountModel>, IAccountRepository
//    {
//        public AccountRepository(Database database) : base(database)
//        {
//        }

//        /// <summary>
//        /// Creates new account
//        /// </summary>
//        /// <param name="account"></param>
//        /// <returns></returns>
//        public async void Create(IAccountModel account)
//        {
//            using (var store = new DocumentStore())
//            {
//                // Open a Session in asynchronous operation mode for cluster-wide transactions

//                SessionOptions options = new SessionOptions
//                {

//                    Database = "neoserver",
//                    TransactionMode = TransactionMode.ClusterWide
//                };
//                store.Urls = new[] { "http://localhost:8080" };
//                store.Initialize();

//                using (IAsyncDocumentSession Session = store.OpenAsyncSession(options))
//                {
//                    await Session.StoreAsync(account);
//                    await Session.SaveChangesAsync();
//                }
//            }
//        }

//        /// <summary>
//        /// Gets account record
//        /// </summary>
//        /// <param name="filter"></param>
//        /// <returns></returns>
//        public AccountModel FirstOrDefault(Expression<Func<AccountModel, bool>> filter)
//        {

//            using (IDocumentSession Session = Database.OpenSession())
//            {
//                return Session.Query<AccountModel>().FirstOrDefault(filter);
//            }

//        }

//        /// <summary>
//        /// Gets account record
//        /// </summary>
//        /// <param name="filter"></param>
//        /// <returns></returns>
//        public IAccountModel Get(string account, string password)
//        {

//            using (IDocumentSession Session = Database.OpenSession())
//            {
//                return Session.Query<AccountModel>().FirstOrDefault(a => a.AccountName == account && a.Password == password);
//            }

//        }

//    }
//}