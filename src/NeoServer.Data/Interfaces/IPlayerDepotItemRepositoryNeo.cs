using NeoServer.Data.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeoServer.Data.Interfaces
{
    public interface IPlayerDepotItemRepositoryNeo : IBaseRepositoryNeo<PlayerDepotItemModel>
    {
        Task<IEnumerable<PlayerDepotItemModel>> GetByPlayerId(uint id);
    }
}
