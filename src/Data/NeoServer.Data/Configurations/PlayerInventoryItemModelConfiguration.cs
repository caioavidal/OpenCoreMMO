using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoServer.Data.Model;
using NeoServer.Data.Seeds;

namespace NeoServer.Data.Configurations
{
    public class PlayerInventoryItemModelConfiguration : IEntityTypeConfiguration<PlayerInventoryItemModel>
    {
        public void Configure(EntityTypeBuilder<PlayerInventoryItemModel> entity)
        {
            entity.HasKey(e => e.Id)
                    .HasName("player_iventory_item_id");

            entity.ToTable("player_inventory_items");

            entity.Property(e => e.PlayerId)
                .HasColumnName("player_id")
                .HasColumnType("int(11)");

            entity.Property(e => e.ServerId)
                .HasColumnName("sid")
                .HasColumnType("int(11)"); 
            
            entity.Property(e => e.SlotId)
                 .HasColumnName("slot_id")
                 .HasColumnType("int(11)");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Amount)
                .HasColumnName("count")
                .HasColumnType("smallint(5)")
                .HasDefaultValueSql("0");

            entity.HasOne(d => d.Player)
                .WithMany(p => p.PlayerInventoryItems)
                .HasForeignKey(d => d.PlayerId)
                .HasConstraintName("player_inventory_items_ibfk_1");

            PlayerInventorySeed.Seed(entity);
        }

   
    }
}
