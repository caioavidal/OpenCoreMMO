using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NeoServer.Data.Configurations;
using NeoServer.Data.Model;
using NeoServer.Server.Model.Players;
using Serilog.Core;

namespace NeoServer.Data
{
    public class NeoContext : DbContext
    {
        private readonly ILoggerFactory logger;
        public NeoContext(DbContextOptions<NeoContext> options, ILoggerFactory logger)
            : base(options)
        {
            this.logger = logger;
        }

        public DbSet<AccountModel> Accounts { get; set; }
        public DbSet<PlayerModel> Players { get; set; }
        public DbSet<PlayerItemModel> PlayerItems { get; set; }
        public DbSet<PlayerDepotItemModel> PlayerDepotItems { get; set; }
        public DbSet<PlayerInventoryItemModel> PlayerInventoryItems { get; set; }
        public DbSet<AccountVipListModel> AccountsVipList { get; set; }
        public DbSet<GuildModel> Guilds { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLoggerFactory(logger);
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
            }

            modelBuilder.ApplyConfiguration(new AccountVipListModelConfiguration());
            modelBuilder.ApplyConfiguration(new GuildMembershipModelConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
