using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoServer.Data.Entities;
using NeoServer.Data.Seeds;

namespace NeoServer.Data.Configurations.ForSqLite;

public class ForSQLitePlayerItemModelConfiguration : IEntityTypeConfiguration<PlayerItemEntity>
{
    public void Configure(EntityTypeBuilder<PlayerItemEntity> entity)
    {
        entity.ToTable("player_items");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("id")
            .HasAnnotation("Sqlite:Autoincrement", true)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Amount)
            .HasColumnName("count")
            .IsRequired()
            .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("1");

        entity.Property(e => e.ParentId)
            .HasColumnName("pid")
            .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

        entity.Property(e => e.PlayerId)
            .HasColumnName("player_id")
            .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

        entity.Property(e => e.ServerId)
            .HasColumnName("sid")
            .IsRequired()
            .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

        entity.Property(e => e.ContainerId)
            .HasColumnName("container_id")
            .IsRequired()
            .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

        entity.HasOne(d => d.Player)
            .WithMany(p => p.PlayerItems)
            .HasForeignKey(d => d.PlayerId)
            .HasConstraintName("player_items_ibfk_1");

        entity.Property(e => e.DecayTo)
            .HasColumnName("decayTo")
            .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

        entity.Property(e => e.DecayDuration)
            .HasColumnName("decayDuration")
            .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

        entity.Property(e => e.DecayElapsed)
            .HasColumnName("decayElapsed")
            .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

        entity.Property(e => e.Charges)
            .HasColumnName("charges")
            .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

        PlayerItemSeed.Seed(entity);
    }
}