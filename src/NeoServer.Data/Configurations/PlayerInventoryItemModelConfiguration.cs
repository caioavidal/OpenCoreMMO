using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoServer.Data.Model;

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

            Seed(entity);
        }

        public void Seed(EntityTypeBuilder<PlayerInventoryItemModel> builder)
        {
            builder.HasData(
                new PlayerInventoryItemModel
                {
                    Id = -1,
                    PlayerId = 1,
                    SlotId = 1,
                    ServerId = 2125,
                    Amount = 1,
                },
                 new PlayerInventoryItemModel
                 {
                     Id = -2,
                     PlayerId = 1,
                     SlotId = 2,
                     ServerId = 2498,
                     Amount = 1,
                 },
                 new PlayerInventoryItemModel
                 {
                     Id = -3,
                     PlayerId = 1,
                     SlotId = 3,
                     ServerId = 1988,
                     Amount = 1,
                 }, new PlayerInventoryItemModel
                 {
                     Id = -4,
                     PlayerId = 1,
                     SlotId = 4,
                     ServerId = 2409,
                     Amount = 1,
                 }, new PlayerInventoryItemModel
                 {
                     Id = -5,
                     PlayerId = 1,
                     SlotId = 5,
                     ServerId = 2466,
                     Amount = 1,
                 },

                //var playerItem1_Right = new PlayerItemModel
                //{
                //    PlayerId = 1,
                //    Pid = 6,
                //    Sid = 1988,
                //    Count = 1,
                //};

                new PlayerInventoryItemModel
                {
                    Id = -6,
                    PlayerId = 1,
                    SlotId = 7,
                    ServerId = 6093,
                    Amount = 1,
                },

                new PlayerInventoryItemModel
                {
                    Id = -7,
                    PlayerId = 1,
                    SlotId = 8,
                    ServerId = 2488,
                    Amount = 1,
                },
                new PlayerInventoryItemModel
                {
                    Id = -8,
                    PlayerId = 1,
                    SlotId = 9,
                    ServerId = 7840,
                    Amount = 1,
                },
                new PlayerInventoryItemModel
                {
                    Id = -9,
                    PlayerId = 1,
                    SlotId = 10,
                    ServerId = 2666,
                    Amount = 1,
                }
            );
        }
    }
}
