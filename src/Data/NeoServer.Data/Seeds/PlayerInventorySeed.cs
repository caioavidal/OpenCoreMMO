using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoServer.Data.Model;
using NeoServer.Game.Common.Creatures.Players;

namespace NeoServer.Data.Seeds
{
    internal sealed class PlayerInventorySeed
    {
        public static void Seed(EntityTypeBuilder<PlayerInventoryItemModel> builder)
        {
            var id = 0;
            for (var playerId = 1; playerId <= 5; playerId++)
                builder.HasData(
                    new PlayerInventoryItemModel
                    {
                        Id = --id,
                        PlayerId = playerId,
                        SlotId = (int) Slot.Head,
                        ServerId = 2457,
                        Amount = 1
                    },
                    new PlayerInventoryItemModel
                    {
                        Id = --id,
                        PlayerId = playerId,
                        SlotId = (int) Slot.Necklace,
                        ServerId = 2173,
                        Amount = 1
                    },
                    new PlayerInventoryItemModel
                    {
                        Id = --id,
                        PlayerId = playerId,
                        SlotId = (int) Slot.Backpack,
                        ServerId = 1988,
                        Amount = 1
                    }, new PlayerInventoryItemModel
                    {
                        Id = --id,
                        PlayerId = playerId,
                        SlotId = (int) Slot.Body,
                        ServerId = 2463,
                        Amount = 1
                    }, new PlayerInventoryItemModel
                    {
                        Id = --id,
                        PlayerId = playerId,
                        SlotId = (int) Slot.Right,
                        ServerId = 2516,
                        Amount = 1
                    },
                    new PlayerInventoryItemModel
                    {
                        Id = --id,
                        PlayerId = playerId,
                        SlotId = (int) Slot.Left,
                        ServerId = 2383,
                        Amount = 1
                    },
                    new PlayerInventoryItemModel
                    {
                        Id = --id,
                        PlayerId = playerId,
                        SlotId = (int) Slot.Legs,
                        ServerId = 2647,
                        Amount = 1
                    },
                    new PlayerInventoryItemModel
                    {
                        Id = --id,
                        PlayerId = playerId,
                        SlotId = (int) Slot.Feet,
                        ServerId = 2643,
                        Amount = 1
                    },
                    new PlayerInventoryItemModel
                    {
                        Id = --id,
                        PlayerId = playerId,
                        SlotId = (int) Slot.Ring,
                        ServerId = 2209,
                        Amount = 1
                    },
                    new PlayerInventoryItemModel
                    {
                        Id = --id,
                        PlayerId = playerId,
                        SlotId = (int) Slot.Ammo,
                        ServerId = 2543,
                        Amount = 70
                    }
                );
        }
    }
}