using Raven.Client.Documents.Indexes;

namespace NeoServer.Data.RavenDB
{

    public class BaseRepository<T> : AbstractIndexCreationTask<T>
    {
        public Database Database { get; }
        public BaseRepository(Database database)
        {
            Database = database;
        }
    }
}