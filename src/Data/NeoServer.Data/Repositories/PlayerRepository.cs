using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using NeoServer.Data.Contexts;
using NeoServer.Data.Interfaces;
using NeoServer.Data.Model;
using Serilog;

namespace NeoServer.Data.Repositories;

public class PlayerRepository : BaseRepository<PlayerModel>, IPlayerRepository
{
    #region constructors

    public PlayerRepository(DbContextOptions<NeoContext> contextOptions, ILogger logger) : base(contextOptions,
        logger)
    {
    }

    #endregion

    public async Task UpdateAllToOffline()
    {
        const string sql = @"UPDATE players SET online = 0";

        await using var context = NewDbContext;

        if (!context.Database.IsRelational()) return;

        await using var connection = context.Database.GetDbConnection();

        await connection.ExecuteAsync(sql);
    }
    
    public async Task Add(PlayerModel player)
    {
        await using var context = NewDbContext;
        await context.AddAsync(player);
        await context.SaveChangesAsync();
    }
}