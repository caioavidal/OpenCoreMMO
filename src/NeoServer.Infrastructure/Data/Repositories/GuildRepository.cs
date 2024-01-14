using Microsoft.EntityFrameworkCore;
using NeoServer.Infrastructure.Data.Contexts;
using NeoServer.Infrastructure.Data.Entities;
using NeoServer.Infrastructure.Data.Interfaces;
using Serilog;

namespace NeoServer.Infrastructure.Data.Repositories;

public class GuildRepository : BaseRepository<GuildEntity>, IGuildRepository
{
    #region constructors

    public GuildRepository(DbContextOptions<NeoContext> contextOptions, ILogger logger) : base(contextOptions,
        logger)
    {
    }

    #endregion

    public async Task<IEnumerable<GuildEntity>> GetAll()
    {
        await using var context = NewDbContext;
        return await context.Guilds.Include(x => x.Members).ThenInclude(x => x.Rank).ToListAsync();
    }
}