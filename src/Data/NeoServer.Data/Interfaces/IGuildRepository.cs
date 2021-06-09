using System.Collections.Generic;
using System.Threading.Tasks;
using NeoServer.Data.Model;

namespace NeoServer.Data.Interfaces
{
    public interface IGuildRepository : IBaseRepositoryNeo<GuildModel>
    {
        Task<IEnumerable<GuildModel>> GetAll();
    }
}