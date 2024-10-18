using Microsoft.EntityFrameworkCore;
using NeoServer.BuildingBlocks.Application.Contracts.Repositories;
using NeoServer.Data.Entities;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Infrastructure.Database.Contexts;
using Serilog;

namespace NeoServer.Infrastructure.Database.Repositories.Player;

/// <summary>
///     Repository class for managing PlayerDepotItem entity.
/// </summary>
public class PlayerDepotItemRepository : BaseRepository<PlayerDepotItemEntity>,
    IPlayerDepotItemRepository
{
    #region constructors

    public PlayerDepotItemRepository(DbContextOptions<NeoContext> contextOptions, ILogger logger) : base(contextOptions,
        logger)
    {
    }

    #endregion

    #region public methods implementation

    public async Task<IEnumerable<PlayerDepotItemEntity>> GetByPlayerId(uint id)
    {
        await using var context = NewDbContext;
        return await context.PlayerDepotItems
            .Where(c => c.PlayerId == id)
            .ToListAsync();
    }

    private static async Task DeleteAll(uint playerId, NeoContext neoContext)
    {
        var items = await neoContext.PlayerDepotItems.Where(x => x.PlayerId == playerId).ToListAsync();
        neoContext.PlayerDepotItems.RemoveRange(items);
    }

    public async Task Save(IPlayer player, IDepot depot)
    {
        await using var context = NewDbContext;

        await DeleteAll(player.Id, context);

        if (depot is null) return;

        await ContainerManager.Save<PlayerDepotItemEntity>(player, depot, context);
        await context.SaveChangesAsync();
    }

    #endregion
}