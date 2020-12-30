using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoServer.Data.Model;
using NeoServer.Server.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Data.Configurations
{
    public class ForSQLitePlayerItemModelConfiguration : IEntityTypeConfiguration<PlayerItemModel>
    {
        public void Configure(EntityTypeBuilder<PlayerItemModel> entity)
        {
            entity.ToTable("player_items");

            //entity.HasIndex(e => e.PlayerId)
            //    .HasDatabaseName("player_id");

            //entity.HasIndex(e => e.ServerId)
            //    .HasDatabaseName("sid");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Attributes)
                .HasColumnName("attributes");

            entity.Property(e => e.Amount)
                .HasColumnName("count")
                .IsRequired()
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("1");

            entity.Property(e => e.Itemtype)
                .HasColumnName("itemtype")
                .HasColumnType("smallint(6)")
                .HasAnnotation("Sqlite:Autoincrement", false).HasDefaultValueSql("0");

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

            entity.HasOne(d => d.Player)
                .WithMany(p => p.PlayerItems)
                .HasForeignKey(d => d.PlayerId)
                .HasConstraintName("player_items_ibfk_1");

            Seed(entity);
        }

        public void Seed(EntityTypeBuilder<PlayerItemModel> builder)
        {
            builder.HasData
                (

          new PlayerItemModel
          {
              Id = -1,
              PlayerId = 1,
              ParentId = 1,
              ServerId = 101,
              Itemtype = 2125,
              Amount = 1,
          },
         new PlayerItemModel
         {
             Id = -2,
             PlayerId = 1,
             ParentId = 2,
             ServerId = 102,
             Itemtype = 2498,
             Amount = 1,
         },
         new PlayerItemModel
         {
             Id = -3,
             PlayerId = 1,
             ParentId = 3,
             ServerId = 103,
             Itemtype = 1988,
             Amount = 1,
         }, new PlayerItemModel
         {
             Id = -4,
             PlayerId = 1,
             ParentId = 4,
             ServerId = 104,
             Itemtype = 2409,
             Amount = 1,
         }, new PlayerItemModel
         {
             Id = -5,
             PlayerId = 1,
             ParentId = 5,
             ServerId = 105,
             Itemtype = 2466,
             Amount = 1,
         },

            //var playerItem1_Right = new PlayerItemModel
            //{
            //    PlayerId = 1,
            //    Pid = 6,
            //    Sid = 1988,
            //    Count = 1,
            //};

            new PlayerItemModel
            {
                Id = -6,
                PlayerId = 1,
                ParentId = 7,
                ServerId = 106,
                Itemtype = 6093,
                Amount = 1,
            },

            new PlayerItemModel
            {
                Id = -7,
                PlayerId = 1,
                ParentId = 8,
                ServerId = 107,
                Itemtype = 2488,
                Amount = 1,
            }, new PlayerItemModel
            {
                Id = -8,
                PlayerId = 1,
                ParentId = 9,
                ServerId = 108,
                Itemtype = 7840,
                Amount = 1,
            }, new PlayerItemModel
            {
                Id = -9,
                PlayerId = 1,
                ParentId = 10,
                ServerId = 109,
                Itemtype = 2666,
                Amount = 1,
            }, new PlayerItemModel
            {
                Id = -10,
                PlayerId = 1,
                ParentId = 103,
                ServerId = 110,
                Itemtype = 1988,
                Amount = 1,
            }
            );
        }
    }
}
