using Microsoft.EntityFrameworkCore;
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
        public DbSet<PlayerModel> Player { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AccountModel>(entity =>
            {
                entity.ToTable("accounts");

                entity.Property(e => e.Id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.AccountName).IsRequired().HasColumnName("name").HasColumnType("varchar(255)");
                entity.Property(e => e.Password).IsRequired().HasColumnName("password").HasColumnType("char(255)");
                entity.Property(e => e.Email).IsRequired().HasColumnName("email").HasColumnType("varchar(255)");
                entity.Property(e => e.PremiumTime).HasColumnName("premdays").HasColumnType("int(11)").HasDefaultValueSql("0");

                entity.HasKey(e => e.Id).HasName("pk_account");
                entity.HasIndex(e => e.AccountName).HasName("uk_name").IsUnique();
                entity.HasIndex(e => e.Email).HasName("uk_email").IsUnique();

                entity.Ignore(e => e.Players);
            });

            modelBuilder.Entity<PlayerModel>(entity =>
            {
                entity.ToTable("players");

                entity.Property(e => e.Id).HasColumnName("id").HasColumnType("int(11)");
                entity.Property(e => e.AccountId).HasColumnName("account_id").HasColumnType("int(11)").HasDefaultValueSql("0");
                entity.Property(e => e.CharacterName).IsRequired().HasColumnName("name").HasColumnType("varchar(255)");

                entity.HasKey(e => e.Id).HasName("pk_players");
                entity.HasIndex(e => e.CharacterName).HasName("uk_name").IsUnique();
                entity.HasOne(d => d.Account).WithMany(p => p.Characters).HasForeignKey(d => d.AccountId).HasConstraintName("players_fk_account");

                entity.Ignore(e => e.Skills);
                entity.Ignore(e => e.Items);
                entity.Ignore(e => e.Outfit);
                entity.Ignore(e => e.Inventory);
            });
        }
    }
}
