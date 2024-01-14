using NeoServer.Infrastructure.Data.Entities;

namespace NeoServer.Infrastructure.Data.Interfaces;

public interface IGuildRepository : IBaseRepositoryNeo<GuildEntity>
{
    Task<IEnumerable<GuildEntity>> GetAll();
}