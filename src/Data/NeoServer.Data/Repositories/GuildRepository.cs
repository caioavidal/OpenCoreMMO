using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NeoServer.Data.Contexts;
using NeoServer.Data.Interfaces;
using NeoServer.Data.Model;
using Serilog;

namespace NeoServer.Data.Repositories
{
    public class GuildRepository : BaseRepository<GuildModel>, IGuildRepository
    {
        #region constructors

        public GuildRepository(DbContextOptions<NeoContext> contextOptions, ILogger logger) : base(contextOptions,
            logger)
        {
        }

        #endregion

        public async Task<IEnumerable<GuildModel>> GetAll()
        {
            await using var context = NewDbContext;
            return await context.Guilds.Include(x => x.Members).ThenInclude(x => x.Rank).ToListAsync();
        }
    }
}