using Microsoft.EntityFrameworkCore;
using NeoServer.Data.Configurations;
using NeoServer.Data.Configurations.ForSqLite;
using NeoServer.Data.Model;
using Serilog;

namespace NeoServer.Data.Contexts;

public class NeoContext : DbContext
{
    private readonly ILogger _logger;

    public NeoContext(DbContextOptions<NeoContext> options, ILogger logger)
        : base(options)
    {
        _logger = logger;
    }

    public DbSet<AccountModel> Accounts { get; set; }
    public DbSet<PlayerModel> Players { get; set; }
    public DbSet<PlayerItemModel> PlayerItems { get; set; }
    public DbSet<PlayerDepotItemModel> PlayerDepotItems { get; set; }
    public DbSet<PlayerInventoryItemModel> PlayerInventoryItems { get; set; }
    public DbSet<AccountVipListModel> AccountsVipList { get; set; }
    public DbSet<GuildModel> Guilds { get; set; }
    public DbSet<GuildMembershipModel> GuildMemberships { get; set; }
    public DbSet<WorldModel> Worlds { get; set; }
    public DbSet<PlayerQuestModel> PlayerQuests { get; set; }
    public DbSet<PlayerOutfitAddonModel> PlayerOutfitAddons { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(m => _logger.Verbose(m),
            (eventId, _) => eventId.Name == $"{DbLoggerCategory.Database.Command.Name}.CommandExecuted");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (Database.IsSqlite())
        {
            modelBuilder.ApplyConfiguration(new ForSQLitePlayerInventoryItemModelConfiguration());
            modelBuilder.ApplyConfiguration(new ForSQLitePlayerDepotItemModelConfiguration());
            modelBuilder.ApplyConfiguration(new ForSQLitePlayerItemModelConfiguration());
            modelBuilder.ApplyConfiguration(new ForSQLitePlayerModelConfiguration());
            modelBuilder.ApplyConfiguration(new ForSQLiteAccountModelConfiguration());
            modelBuilder.ApplyConfiguration(new ForSQLiteGuildModelConfiguration());
            modelBuilder.ApplyConfiguration(new ForSQLiteGuildRankModelConfiguration());
            modelBuilder.ApplyConfiguration(new ForSQLiteWorldModelConfiguration());
            modelBuilder.ApplyConfiguration(new ForSQLitePlayerQuestModelConfiguration());
            modelBuilder.ApplyConfiguration(new ForSQLitePlayerOutfitAddonModelConfiguration());
        }
        else
        {
            modelBuilder.ApplyConfiguration(new PlayerInventoryItemModelConfiguration());
            modelBuilder.ApplyConfiguration(new PlayerDepotItemModelConfiguration());
            modelBuilder.ApplyConfiguration(new PlayerItemModelConfiguration());
            modelBuilder.ApplyConfiguration(new PlayerModelConfiguration());
            modelBuilder.ApplyConfiguration(new AccountModelConfiguration());
            modelBuilder.ApplyConfiguration(new GuildModelConfiguration());
            modelBuilder.ApplyConfiguration(new GuildRankModelConfiguration());
            modelBuilder.ApplyConfiguration(new WorldModelConfiguration());
            modelBuilder.ApplyConfiguration(new PlayerQuestModelConfiguration());
            modelBuilder.ApplyConfiguration(new PlayerOutfitAddonModelConfiguration());
        }

        modelBuilder.ApplyConfiguration(new AccountVipListModelConfiguration());
        modelBuilder.ApplyConfiguration(new GuildMembershipModelConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}