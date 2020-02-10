using Raven.Client.Documents;
using Raven.Client.Documents.Session;

public class Database
{

    private static DocumentStore store;
    public static void Connect()
    {
        store = new DocumentStore();
        store.Urls = new[] { "http://localhost:8080" }; //todo: add to appsettings
        store.Initialize();
    }

    public static IAsyncDocumentSession OpenAsyncSession()
    {
        SessionOptions options = new SessionOptions
        {

            Database = "neoserver",
            TransactionMode = TransactionMode.ClusterWide
        };
        return store.OpenAsyncSession(options);
    }
}