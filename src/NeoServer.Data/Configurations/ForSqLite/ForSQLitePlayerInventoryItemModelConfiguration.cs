using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoServer.Data.Model;

namespace NeoServer.Data.Configurations
{
    public class ForSQLitePlayerInventoryItemModelConfiguration : IEntityTypeConfiguration<PlayerInventoryItemModel>
    {
        public void Configure(EntityTypeBuilder<PlayerInventoryItemModel> entity)
        {
            entity.ToTable("player_inventory_items");

            //entity.HasIndex(e => e.PlayerId)
            //    .HasDatabaseName("player_id");

            //entity.HasIndex(e => e.ServerId)
            //    .HasDatabaseName("sid");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Amount)
                .HasColumnName("count")
                .IsRequired()
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("1");

            entity.Property(e => e.SlotId)
                .HasColumnName("pid")
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
