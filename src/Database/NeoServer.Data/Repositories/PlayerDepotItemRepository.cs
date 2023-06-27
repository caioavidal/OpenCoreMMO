using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using NeoServer.Data.Contexts;
using NeoServer.Data.Interfaces;
using NeoServer.Data.Model;
using NeoServer.Data.Parsers;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using Serilog;

namespace NeoServer.Data.Repositories;

/// <summary>
/// Repository class for managing PlayerDepotItem entity.
/// </summary>
public class PlayerDepotItemRepository : BaseRepository<PlayerDepotItemModel>,
    IPlayerDepotItemRepository
{
    #region constructors

    public PlayerDepotItemRepository(DbContextOptions<NeoContext> contextOptions, ILogger logger) : base(contextOptions,
        logger)
    {
    }

    #endregion

    #region public methods implementation

    public async Task<IEnumerable<PlayerDepotItemModel>> GetByPlayerId(uint id)
    {
        await using var context = NewDbContext;
        return await context.PlayerDepotItems
            .Where(c => c.PlayerId == id)
            .ToListAsync();
    }

    public async Task DeleteAll(uint playerId)
    {
        await using var context = NewDbContext;
        if (!context.Database.IsRelational())
        {
            var items = context.PlayerDepotItems.Where(x => x.PlayerId == playerId);
            foreach (var item in items) await Delete(item);
            return;
        }

        await using var connection = context.Database.GetDbConnection();

        await connection.ExecuteAsync("delete from player_depot_items where player_id = @playerId", new { playerId });
    }

    public async Task Save(int playerId, IDepot depot) => await Save(playerId, depot.Items);

    #endregion

    private async Task Save(int playerId, List<IItem> items, int parentId = 0)
    {
        foreach (var item in items)
        {
            var itemModel = ItemModelParser.ToModel(item);
            itemModel.PlayerId = playerId;
            itemModel.ParentId = parentId;
            await Insert(itemModel);

            if (item is IContainer container) await Save(playerId, container.Items, itemModel.Id);
        }
    }
}