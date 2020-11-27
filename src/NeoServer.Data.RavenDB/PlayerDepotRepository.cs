using NeoServer.Data.Model;
using NeoServer.Data.Repositories;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Data.RavenDB
{
    public class PlayerDepotRepository : BaseRepository<PlayerDepotModel>, IPlayerDepotRepository
    {
        public PlayerDepotRepository(Database database) : base(database) { }

        public async Task<PlayerDepotModel> Get(uint playerId)
        {
            using (IAsyncDocumentSession Session = Database.OpenAsyncSession())
            {
                return await Session.Query<PlayerDepotModel>().FirstOrDefaultAsync(a => a.PlayerId == playerId);
            }
        }
        public async void Save(PlayerDepotModel model)
        {
            using (IAsyncDocumentSession Session = Database.OpenAsyncSession())
            {
                await Session.StoreAsync(model);
                await Session.SaveChangesAsync();
            }
        }
    }
}
