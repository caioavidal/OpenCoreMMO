using Microsoft.EntityFrameworkCore;
using NeoServer.Data.Configurations;
using NeoServer.Data.Model;
using NeoServer.Server.Model.Players;

namespace NeoServer.Data
{
    public class NeoContext : DbContext
    {
        public NeoContext(DbContextOptions<NeoContext> options)
            : base(options)
        { }

        public DbSet<AccountModel> Accounts { get; set; }
        public DbSet<PlayerModel> Players { get; set; }
        public DbSet<PlayerItemModel> PlayerItems { get; set; }
        public DbSet<PlayerDepotItemModel> PlayerDepotItems { get; set; }
        public DbSet<PlayerInventoryItemModel> PlayerInventoryItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(AccountModelConfiguration)));

            if (Database.IsSqlite())
            {
                modelBuilder.ApplyConfiguration(new ForSQLitePlayerInventoryItemModelConfiguration());
                modelBuilder.ApplyConfiguration(new ForSQLitePlayerDepotItemModelConfiguration());
                modelBuilder.ApplyConfiguration(new ForSQLitePlayerItemModelConfiguration());
                modelBuilder.ApplyConfiguration(new ForSQLitePlayerModelConfiguration());
                modelBuilder.ApplyConfiguration(new ForSQLiteAccountModelConfiguration());
            }
            else
            {
                modelBuilder.ApplyConfiguration(new PlayerInventoryItemModelConfiguration());
                modelBuilder.ApplyConfiguration(new PlayerDepotItemModelConfiguration());
                modelBuilder.ApplyConfiguration(new PlayerItemModelConfiguration());
                modelBuilder.ApplyConfiguration(new PlayerModelConfiguration());
                modelBuilder.ApplyConfiguration(new AccountModelConfiguration());
            }
            base.OnModelCreating(modelBuilder);
        }
    }
}
