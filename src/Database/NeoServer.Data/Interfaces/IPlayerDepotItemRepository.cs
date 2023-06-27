using System.Collections.Generic;
using System.Threading.Tasks;
using NeoServer.Data.Model;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;

namespace NeoServer.Data.Interfaces;

public interface IPlayerDepotItemRepository : IBaseRepositoryNeo<PlayerItemModel>
{
    Task DeleteAll(uint playerId);
    Task<IEnumerable<PlayerItemModel>> GetByPlayerId(uint id);
    Task Save(IPlayer player, IDepot depot);
}