using System.Collections.Generic;
using System.Threading.Tasks;
using NeoServer.Data.Entities;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;

namespace NeoServer.Data.Interfaces;

public interface IPlayerDepotItemRepository : IBaseRepositoryNeo<PlayerDepotItemEntity>
{
    Task<IEnumerable<PlayerDepotItemEntity>> GetByPlayerId(uint id);
    Task Save(IPlayer player, IDepot depot);
}