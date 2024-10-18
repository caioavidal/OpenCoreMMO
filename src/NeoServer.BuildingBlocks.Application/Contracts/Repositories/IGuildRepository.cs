using NeoServer.Data.Entities;

namespace NeoServer.BuildingBlocks.Application.Contracts.Repositories;

public interface IGuildRepository : IBaseRepositoryNeo<GuildEntity>
{
    Task<IEnumerable<GuildEntity>> GetAll();
}