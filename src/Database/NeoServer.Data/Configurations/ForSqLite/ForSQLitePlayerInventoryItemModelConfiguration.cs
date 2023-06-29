using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoServer.Data.Entities;
using NeoServer.Data.Seeds;

namespace NeoServer.Data.Configurations.ForSqLite;

public class ForSQLitePlayerInventoryItemModelConfiguration : IEntityTypeConfiguration<PlayerInventoryItemEntity>
{
    public void Configure(EntityTypeBuilder<PlayerInventoryItemEntity> entity)
    {
        entity.ToTable("player_inventory_items");


        entity.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Amount)
            .HasColumnName("count")
            .IsRequired()
            .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("1");

        entity.Property(e => e.SlotId)
            .HasColumnName("slot_id")
            .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("1");

        entity.Property(e => e.PlayerId)
            .HasColumnName("player_id")
            .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

        entity.Property(e => e.ServerId)
            .HasColumnName("sid")
            .IsRequired()
            .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

        entity.HasOne(d => d.Player)
            .WithMany(p => p.PlayerInventoryItems)
            .HasForeignKey(d => d.PlayerId)
            .HasConstraintName("player_inventory_items_ibfk_1");

        PlayerInventorySeed.Seed(entity);
    }
}