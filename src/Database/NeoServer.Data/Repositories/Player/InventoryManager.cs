using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using NeoServer.Data.Contexts;
using NeoServer.Data.Entities;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Helpers;

namespace NeoServer.Data.Repositories.Player;

internal class InventoryManager
{
    private readonly ContainerManager<PlayerEntity> _containerManager;
    private readonly PlayerRepository _playerRepository;

    public InventoryManager(PlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
        _containerManager = new ContainerManager<PlayerEntity>(_playerRepository);
    }

    public async Task SaveBackpack(IPlayer player, NeoContext neoContext)
    {
        if (Guard.AnyNull(player, player.Inventory?.BackpackSlot)) return;

        if (player.Inventory?.BackpackSlot?.Items?.Count == 0) return;
        
        neoContext.PlayerItems.RemoveRange(neoContext.PlayerItems.Where(x => x.PlayerId == player.Id));

        await _containerManager.Save<PlayerItemEntity>(player, player.Inventory?.BackpackSlot, neoContext);
    }

    public async Task SavePlayerInventory(IPlayer player, NeoContext neoContext)
    {
        var playerInventory = await neoContext
            .PlayerInventoryItems
            .Where(x => x.PlayerId == player.Id)
            .ToDictionaryAsync(x=>x.SlotId);

        foreach (var slot in new[]
                 {
                     Slot.Necklace, Slot.Head, Slot.Backpack, Slot.Left, Slot.Body, Slot.Right, Slot.Ring, Slot.Legs,
                     Slot.Ammo, Slot.Feet
                 })
        {
            var item = player.Inventory[slot];

            if (playerInventory.TryGetValue((int)slot, out var playerInventoryItemEntity))
            {
                playerInventoryItemEntity.ServerId = item?.Metadata?.TypeId ?? 0;
                playerInventoryItemEntity.Amount = item?.Amount ?? 0;
                playerInventoryItemEntity.PlayerId = (int)player.Id;
                playerInventoryItemEntity.SlotId = (int)slot;
                
                neoContext.PlayerInventoryItems.Update(playerInventoryItemEntity);
                continue;
            }
            
            await neoContext.PlayerInventoryItems.AddAsync(new PlayerInventoryItemEntity
            {
                Amount = 0, PlayerId = (int)player.Id, SlotId = (int)slot, ServerId = 0
            });
        }
    }

    public List<Task> UpdatePlayerInventory(IPlayer player, NeoContext neoContext)
    {
        if (player is null) return null;

        const string sql = @"UPDATE player_inventory_items
                           SET sid = @sid,
                               count = @count
                         WHERE player_id = @playerId and slot_id = @pid";
        

        var tasks = new List<Task>();

        foreach (var slot in new[]
                 {
                     Slot.Necklace, Slot.Head, Slot.Backpack, Slot.Left, Slot.Body, Slot.Right, Slot.Ring, Slot.Legs,
                     Slot.Ammo, Slot.Feet
                 })
            tasks.Add(Task.Run(async () =>
            {
                await using var context = _playerRepository.NewDbContext;
                if (!context.Database.IsRelational()) return;

                await using var connection = context.Database.GetDbConnection();

                var item = player.Inventory[slot];

                await connection.ExecuteAsync(sql, new
                {
                    sid = item?.Metadata?.TypeId ?? 0,
                    count = item?.Amount ?? 0,
                    playerId = player.Id,
                    pid = (int)slot
                }, commandTimeout: 5);
            }));

        return tasks;
    }

    // private async Task AddMissingInventoryRecords(IPlayer player)
    // {
    //     await using var context = _playerRepository.NewDbContext;
    //
    //     var inventoryRecords = context.PlayerInventoryItems
    //         .Where(x => x.PlayerId == player.Id)
    //         .Select(x => x.SlotId)
    //         .ToHashSet();
    //
    //     for (var i = 1; i <= 10; i++)
    //     {
    //         if (inventoryRecords.Contains(i)) continue;
    //
    //         await context.PlayerInventoryItems.AddAsync(new PlayerInventoryItemEntity
    //         {
    //             Amount = 0, PlayerId = (int)player.Id, SlotId = i, ServerId = 0
    //         });
    //     }
    //
    //     await context.SaveChangesAsync();
    // }
}