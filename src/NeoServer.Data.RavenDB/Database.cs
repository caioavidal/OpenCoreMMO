using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace NeoServer.Data.RavenDB
{
    public class Database
    {

        private DocumentStore store;
        /// <summary>
        /// Connects to database
        /// </summary>
        public void Connect()
        {
            store = new DocumentStore();
            store.Urls = new[] { "http://localhost:8080" }; //todo: add to appsettings
            store.Initialize();
        }

        public IAsyncDocumentSession OpenAsyncSession()
        {
            SessionOptions options = new SessionOptions
            {

                Database = "neoserver",
                TransactionMode = TransactionMode.ClusterWide
            };
            return store.OpenAsyncSession(options);
        }
    }
}