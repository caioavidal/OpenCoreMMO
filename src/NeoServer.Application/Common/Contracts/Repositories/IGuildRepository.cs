using NeoServer.Data.Entities;

namespace NeoServer.Application.Repositories;

public interface IGuildRepository : IBaseRepositoryNeo<GuildEntity>
{
    Task<IEnumerable<GuildEntity>> GetAll();
}