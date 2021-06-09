using System.Collections.Generic;
using System.Threading.Tasks;
using NeoServer.Data.Model;

namespace NeoServer.Data.Interfaces
{
    public interface IPlayerDepotItemRepositoryNeo : IBaseRepositoryNeo<PlayerDepotItemModel>
    {
        Task DeleteAll(uint playerId);
        Task<IEnumerable<PlayerDepotItemModel>> GetByPlayerId(uint id);
    }
}