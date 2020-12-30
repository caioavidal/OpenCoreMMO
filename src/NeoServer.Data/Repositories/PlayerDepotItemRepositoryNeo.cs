using Microsoft.EntityFrameworkCore;
using NeoServer.Data.Interfaces;
using NeoServer.Data.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeoServer.Data.Repositories
{
    public class PlayerDepotItemRepositoryNeo : BaseRepositoryNeo<PlayerDepotItemModel, NeoContext>, IPlayerDepotItemRepositoryNeo
    {
        #region constructors

        public PlayerDepotItemRepositoryNeo(NeoContext context) : base(context) { }

        #endregion

        #region public methods implementation

        public async Task<IEnumerable<PlayerDepotItemModel>> GetByPlayerId(uint id)
        {
            return await GetContext.PlayerDepotItems
                .Where(c => c.Player.Equals(id))
                .ToListAsync();
        }

        #endregion
    }
}
