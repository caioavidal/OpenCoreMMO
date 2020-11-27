using NeoServer.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Data.Repositories
{
    public interface IPlayerDepotRepository
    {
        Task<PlayerDepotModel> Get(uint playerId);
        void Save(PlayerDepotModel model);
    }
}
