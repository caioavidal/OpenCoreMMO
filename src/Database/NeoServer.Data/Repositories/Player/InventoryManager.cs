using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using NeoServer.Data.Model;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Helpers;

namespace NeoServer.Data.Repositories.Player;

internal class InventoryManager
{
    private readonly PlayerRepository _playerRepository;
    private readonly ContainerManager _containerManager;

    public InventoryManager(PlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
        _containerManager = new ContainerManager(_playerRepository.NewDbContext);
    }
    public async Task SaveBackpack(IPlayer player)
    {
        if (Guard.AnyNull(player, player.Inventory?.BackpackSlot)) return;

        if (player.Inventory?.BackpackSlot?.Items?.Count == 0) return;

        await using var context = _playerRepository.NewDbContext;

        context.PlayerItems.RemoveRange(context.PlayerItems.Where(x => x.PlayerId == player.Id));

        await _containerManager.Save(player, player.Inventory?.BackpackSlot);
    }
    
    public async Task SavePlayerInventory(IPlayer player)
    {
        await AddMissingInventoryRecords(player);

        if (UpdatePlayerInventory(player) is { } updates) await Task.WhenAll(updates);
    }
    
    public List<Task> UpdatePlayerInventory(IPlayer player)
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
    
    private async Task AddMissingInventoryRecords(IPlayer player)
    {
        await using var context = _playerRepository.NewDbContext;

        var inventoryRecords = context.PlayerInventoryItems
            .Where(x => x.PlayerId == player.Id)
            .Select(x => x.SlotId)
            .ToHashSet();

        for (var i = 1; i <= 10; i++)
        {
            if (inventoryRecords.Contains(i)) continue;

            await context.PlayerInventoryItems.AddAsync(new PlayerInventoryItemModel
            {
                Amount = 0, PlayerId = (int)player.Id, SlotId = i, ServerId = 0
            });
        }

        await context.SaveChangesAsync();
    }
}