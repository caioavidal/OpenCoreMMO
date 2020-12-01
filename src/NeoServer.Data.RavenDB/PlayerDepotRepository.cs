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

        public PlayerDepotModel Get(uint playerId)
        {
            using (IDocumentSession Session = Database.OpenSession())
            {
                return Session.Query<PlayerDepotModel>().FirstOrDefault(a => a.PlayerId == playerId);
            }
        }
        public void Save(PlayerDepotModel model)
        {

            using (IDocumentSession Session = Database.OpenSession())
            {
                var record = Get(model.PlayerId);
                if(record is not null) model.Id = record.Id;

                Session.Store(model);
                Session.SaveChanges();
            }
        }
    }
}
