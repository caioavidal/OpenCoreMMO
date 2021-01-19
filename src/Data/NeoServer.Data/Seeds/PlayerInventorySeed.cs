using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoServer.Data.Model;
using NeoServer.Game.Common.Players;

namespace NeoServer.Data.Seeds
{
    internal sealed class PlayerInventorySeed
    {
        public static void Seed(EntityTypeBuilder<PlayerInventoryItemModel> builder)
        {
            builder.HasData(
                new PlayerInventoryItemModel
                {
                    Id = -1,
                    PlayerId = 1,
                    SlotId = (int)Slot.Head,
                    ServerId = 2457,
                    Amount = 1,
                },
                 new PlayerInventoryItemModel
                 {
                     Id = -2,
                     PlayerId = 1,
                     SlotId = (int)Slot.Necklace,
                     ServerId = 2173,
                     Amount = 1,
                 },
                 new PlayerInventoryItemModel
                 {
                     Id = -3,
                     PlayerId = 1,
                     SlotId = (int)Slot.Backpack,
                     ServerId = 1988,
                     Amount = 1,
                 }, new PlayerInventoryItemModel
                 {
                     Id = -4,
                     PlayerId = 1,
                     SlotId = (int)Slot.Body,
                     ServerId = 2463,
                     Amount = 1,
                 }, new PlayerInventoryItemModel
                 {
                     Id = -5,
                     PlayerId = 1,
                     SlotId = (int)Slot.Right,
                     ServerId = 2516,
                     Amount = 1,
                 },

              new PlayerInventoryItemModel
              {
                  Id = -6,
                  PlayerId = 1,
                  SlotId = (int)Slot.Left,
                  ServerId = 2383,
                  Amount = 1,
              },

            new PlayerInventoryItemModel
            {
                Id = -7,
                PlayerId = 1,
                SlotId = (int)Slot.Legs,
                ServerId = 2647,
                Amount = 1,
            },

                new PlayerInventoryItemModel
                {
                    Id = -8,
                    PlayerId = 1,
                    SlotId = (int)Slot.Feet,
                    ServerId = 2643,
                    Amount = 1,
                },
                new PlayerInventoryItemModel
                {
                    Id = -9,
                    PlayerId = 1,
                    SlotId = (int)Slot.Ring,
                    ServerId = 2209,
                    Amount = 1,
                },
                new PlayerInventoryItemModel
                {
                    Id = -10,
                    PlayerId = 1,
                    SlotId = (int)Slot.Ammo,
                    ServerId = 2543,
                    Amount = 70,
                }
            );
        }
    }
}
