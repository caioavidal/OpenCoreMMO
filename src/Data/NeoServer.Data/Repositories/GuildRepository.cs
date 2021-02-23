using Microsoft.EntityFrameworkCore;
using NeoServer.Data.Interfaces;
using NeoServer.Data.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeoServer.Data.Repositories
{
    public class GuildRepository : BaseRepository<GuildModel, NeoContext>, IGuildRepository
    {
        #region constructors

        public GuildRepository(NeoContext context) : base(context) { }

        #endregion

        public async Task<IEnumerable<GuildModel>> GetAll()
        {
            return await Context.Guilds.Include(x => x.Members).ThenInclude(x => x.Rank).ToListAsync();
        }
    }
}
