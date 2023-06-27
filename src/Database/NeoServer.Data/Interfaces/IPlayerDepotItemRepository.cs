using System.Collections.Generic;
using System.Threading.Tasks;
using NeoServer.Data.Model;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;

namespace NeoServer.Data.Interfaces;

public interface IPlayerDepotItemRepository : IBaseRepositoryNeo<PlayerDepotItemModel>
{
    Task DeleteAll(uint playerId);
    Task<IEnumerable<PlayerDepotItemModel>> GetByPlayerId(uint id);
    Task Save(int playerId, IDepot depot);
}