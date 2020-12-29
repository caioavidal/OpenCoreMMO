using Microsoft.EntityFrameworkCore;
using NeoServer.Data.Configurations;
using NeoServer.Data.Model;
using NeoServer.Server.Model.Players;
using System.Reflection;

namespace NeoServer.Data
{
    public class NeoContext : DbContext
    {
        public NeoContext(DbContextOptions<NeoContext> options)
            : base(options)
        { }

        public DbSet<AccountModel> Accounts { get; set; }
        public DbSet<PlayerModel> Player { get; set; }
        public DbSet<PlayerItemModel> PlayerItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(AccountModelConfiguration)));
        }
    }
}
