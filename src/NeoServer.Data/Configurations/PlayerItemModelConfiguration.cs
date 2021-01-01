using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoServer.Data.Model;

namespace NeoServer.Data.Configurations
{
    public class PlayerItemModelConfiguration : IEntityTypeConfiguration<PlayerItemModel>
    {
        public void Configure(EntityTypeBuilder<PlayerItemModel> entity)
        {
            entity.ToTable("player_items");

            entity.HasIndex(e => e.PlayerId)
                .HasDatabaseName("player_id");

            entity.HasIndex(e => e.ServerId)
                .HasDatabaseName("sid");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
            //.HasColumnType("int(11)");

            entity.Property(e => e.Amount)
                .HasColumnName("count")
                .IsRequired()
                .HasColumnType("smallint(5)")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("1");

            entity.Property(e => e.ParentId)
                .HasColumnName("pid")
                .HasColumnType("int(11)")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

            entity.Property(e => e.PlayerId)
                .HasColumnName("player_id")
                .HasColumnType("int(11)")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

            entity.Property(e => e.ServerId)
                .HasColumnName("sid")
                .IsRequired()
                .HasColumnType("int(11)")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

            entity.HasOne(d => d.Player)
                .WithMany(p => p.PlayerItems)
                .HasForeignKey(d => d.PlayerId)
                .HasConstraintName("player_items_ibfk_1");

            Seed(entity);
        }

        public void Seed(EntityTypeBuilder<PlayerItemModel> builder)
        {
            builder.HasData(
                new PlayerItemModel
                {
                    Id = -10,
                    PlayerId = 1,
                    ParentId = 0,
                    ServerId = 1988,
                    Amount = 1,
                }
            );
        }
    }
}
