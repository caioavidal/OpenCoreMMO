using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoServer.Data.Model;
using NeoServer.Game.Common.Creatures.Players;

namespace NeoServer.Data.Seeds;

internal static class PlayerInventorySeed
{
    public static void Seed(EntityTypeBuilder<PlayerInventoryItemModel> builder)
    {
        var id = 0;

        SeedGod(builder, ref id);
        SeedSorcerer(builder, ref id);
        SeedKnight(builder, ref id);
        SeedDruid(builder, ref id);
        SeedPaladin(builder, ref id);
    }

    private static void SeedSorcerer(EntityTypeBuilder<PlayerInventoryItemModel> builder, ref int id)
    {
        var playerId = 2;
        builder.HasData(
            new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Head,
                ServerId = 8820,
                Amount = 1
            },
            new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Backpack,
                ServerId = 1988,
                Amount = 1
            }, new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Body,
                ServerId = 8819,
                Amount = 1
            }, new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Right,
                ServerId = 2175,
                Amount = 1
            },
            new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Left,
                ServerId = 2190,
                Amount = 1
            },
            new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Legs,
                ServerId = 2649,
                Amount = 1
            },
            new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Feet,
                ServerId = 2643,
                Amount = 1
            }
        );
    }

    private static void SeedDruid(EntityTypeBuilder<PlayerInventoryItemModel> builder, ref int id)
    {
        var playerId = 4;
        builder.HasData(
            new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Head,
                ServerId = 8820,
                Amount = 1
            },
            new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Backpack,
                ServerId = 1988,
                Amount = 1
            }, new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Body,
                ServerId = 8819,
                Amount = 1
            }, new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Right,
                ServerId = 2175,
                Amount = 1
            },
            new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Left,
                ServerId = 2182,
                Amount = 1
            },
            new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Legs,
                ServerId = 2649,
                Amount = 1
            },
            new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Feet,
                ServerId = 2643,
                Amount = 1
            }
        );
    }

    private static void SeedKnight(EntityTypeBuilder<PlayerInventoryItemModel> builder, ref int id)
    {
        var playerId = 3;
        builder.HasData(
            new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Head,
                ServerId = 2457,
                Amount = 1
            },
            new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Backpack,
                ServerId = 1988,
                Amount = 1
            }, new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Body,
                ServerId = 2465,
                Amount = 1
            }, new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Right,
                ServerId = 2509,
                Amount = 1
            },
            new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Left,
                ServerId = 8602,
                Amount = 1
            },
            new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Legs,
                ServerId = 2478,
                Amount = 1
            },
            new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Feet,
                ServerId = 2643,
                Amount = 1
            }
        );
    }

    private static void SeedPaladin(EntityTypeBuilder<PlayerInventoryItemModel> builder, ref int id)
    {
        var playerId = 5;
        builder.HasData(
            new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Head,
                ServerId = 2461,
                Amount = 1
            },
            new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Backpack,
                ServerId = 1988,
                Amount = 1
            }, new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Body,
                ServerId = 2660,
                Amount = 1
            },
            new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Left,
                ServerId = 2456,
                Amount = 1
            },
            new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Legs,
                ServerId = 8923,
                Amount = 1
            },
            new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Feet,
                ServerId = 2643,
                Amount = 1
            },
            new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Ammo,
                ServerId = 2544,
                Amount = 100
            }
        );
    }

    private static void SeedGod(EntityTypeBuilder<PlayerInventoryItemModel> builder, ref int id)
    {
        var playerId = 1;
        builder.HasData(
            new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Head,
                ServerId = 2457,
                Amount = 1
            },
            new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Necklace,
                ServerId = 2173,
                Amount = 1
            },
            new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Backpack,
                ServerId = 1988,
                Amount = 1
            }, new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Body,
                ServerId = 2463,
                Amount = 1
            }, new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Right,
                ServerId = 2516,
                Amount = 1
            },
            new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Left,
                ServerId = 2383,
                Amount = 1
            },
            new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Legs,
                ServerId = 2647,
                Amount = 1
            },
            new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Feet,
                ServerId = 2643,
                Amount = 1
            },
            new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Ring,
                ServerId = 2209,
                Amount = 1
            },
            new PlayerInventoryItemModel
            {
                Id = --id,
                PlayerId = playerId,
                SlotId = (int)Slot.Ammo,
                ServerId = 2543,
                Amount = 70
            }
        );
    }
}