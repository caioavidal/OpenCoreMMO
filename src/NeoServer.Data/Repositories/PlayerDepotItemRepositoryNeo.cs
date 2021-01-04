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
            return await Context.PlayerDepotItems
                .Where(c => c.PlayerId == id)
                .ToListAsync();
        }

        public async Task DeleteAll(uint playerId)
        {
            if (!Context.Database.IsRelational())
            {
                var items = Context.PlayerDepotItems.Where(x => x.PlayerId == playerId);
                foreach (var item in items)
                {
                    await Delete(item);
                }
                return;
            }
            await Context.Database.ExecuteSqlRawAsync($"delete from player_depot_items where player_id = {playerId}");
        }


        #endregion
    }
}
