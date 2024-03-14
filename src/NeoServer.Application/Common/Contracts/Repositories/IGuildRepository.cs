using NeoServer.Data.Entities;

namespace NeoServer.Application.Common.Contracts.Repositories;

public interface IGuildRepository : IBaseRepositoryNeo<GuildEntity>
{
    Task<IEnumerable<GuildEntity>> GetAll();
}