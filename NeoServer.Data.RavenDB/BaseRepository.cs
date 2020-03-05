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

    public class BaseRepository<T> : AbstractIndexCreationTask<T>
    {
        public Database Database { get; }
        public BaseRepository(Database database)
        {
            Database = database;
        }
    }
}