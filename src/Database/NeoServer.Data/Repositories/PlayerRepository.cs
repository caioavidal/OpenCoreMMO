using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using NeoServer.Data.Contexts;
using NeoServer.Data.Interfaces;
using NeoServer.Data.Model;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Helpers;
using Serilog;

namespace NeoServer.Data.Repositories;

public class PlayerRepository : BaseRepository<PlayerModel>, IPlayerRepository
{
    #region constructors

    public PlayerRepository(DbContextOptions<NeoContext> contextOptions, ILogger logger) : base(contextOptions,
        logger)
    {
    }

    #endregion

    public async Task UpdateAllToOffline()
    {
        const string sql = @"UPDATE players SET online = 0";

        await using var context = NewDbContext;

        if (!context.Database.IsRelational()) return;

        await using var connection = context.Database.GetDbConnection();

        await connection.ExecuteAsync(sql);
    }

    public async Task Add(PlayerModel player)
    {
        await using var context = NewDbContext;
        await context.AddAsync(player);
        await context.SaveChangesAsync();
    }

    public async Task<List<PlayerOutfitAddonModel>> GetOutfitAddons(int playerId)
    {
        await using var context = NewDbContext;
        return await context.PlayerOutfitAddons.Where(x => x.PlayerId == playerId).ToListAsync();
    }

    public async Task SaveBackpack(IPlayer player)
    {
        if (Guard.AnyNull(player, player.Inventory?.BackpackSlot)) return;

        if (player.Inventory?.BackpackSlot?.Items?.Count == 0) return;

        await using var context = NewDbContext;

        context.PlayerItems.RemoveRange(context.PlayerItems.Where(x => x.PlayerId == player.Id));

        var containers = new Queue<(IContainer Container, int ParentId)>();
        containers.Enqueue((player.Inventory?.BackpackSlot, 0));

        while(containers.TryDequeue(out var container))
        {
            var items = container.Container.Items;
            if (!items.Any()) continue;
            
            foreach (var item in items)
            {
                var itemModel = new PlayerItemModel
                {
                    ServerId = (short)item.Metadata.TypeId,
                    Amount = item is ICumulative cumulative ? cumulative.Amount : (short)1,
                    PlayerId = (int)player.Id,
                    ParentId = container.ParentId
                    // DecayTo = item.Decay?.DecaysTo,
                    // DecayDuration = item.Decay?.Duration,
                    // DecayElapsed = item.Decay?.Elapsed,
                    // Charges = item is IChargeable chargeable ? chargeable.Charges : null
                };

                await context.PlayerItems.AddAsync(itemModel);

                if (item is IContainer innerContainer) // await SaveBackpack(player, container.Items, itemModel.Id);
                {
                    containers.Enqueue((innerContainer, itemModel.Id));
                }
            }
        } 

        await CommitChanges(context);
    }
    
    public async Task<PlayerModel> GetPlayer(string playerName)
    {
        await using var context = NewDbContext;
        return await context.Players.FirstOrDefaultAsync(x => x.Name.Equals(playerName));
    }

    public async Task UpdatePlayer(IPlayer player)
    {
        const string sql = @"UPDATE players
                           SET 
                               cap = @cap,
                               level = @level,
                               mana = @mana,
                               manamax = @manamax,
                               health = @health,
                               healthmax = @healthmax,
                               Soul = @Soul,
                               MaxSoul = @MaxSoul,
                               Speed = @Speed,
                               StaminaMinutes = @StaminaMinutes,
                               -- Online = @Online,
                               lookaddons = @lookaddons,
                               lookbody = @lookbody,
                               lookfeet = @lookfeet,
                               lookhead = @lookhead,
                               looklegs = @looklegs,
                               looktype = @looktype,
                               posx = @posx,
                               posy = @posy,
                               posz = @posz,
                               -- offlinetraining_time = @offlinetraining_time,
                               -- offlinetraining_skill = @offlinetraining_skill,
                               skill_fist = @skill_fist,
                               skill_fist_tries = @skill_fist_tries,
                               skill_club = @skill_club,
                               skill_club_tries = @skill_club_tries,
                               skill_sword = @skill_sword,
                               skill_sword_tries = @skill_sword_tries,
                               skill_axe = @skill_axe,
                               skill_axe_tries = @skill_axe_tries,
                               skill_dist = @skill_dist,
                               skill_dist_tries = @skill_dist_tries,
                               skill_shielding = @skill_shielding,
                               skill_shielding_tries = @skill_shielding_tries,
                               skill_fishing = @skill_fishing,
                               skill_fishing_tries = @skill_fishing_tries,
                               MagicLevel = @MagicLevel,
                               MagicLevelTries = @MagicLevelTries,
                               Experience = @Experience,
                               ChaseMode = @ChaseMode,
                               FightMode = @FightMode,
                               vocation = @vocation,
                               remaining_recovery_seconds = @remaining_recovery_seconds
                         WHERE id = @playerId";

        await using var context = NewDbContext;

        if (!context.Database.IsRelational()) return;

        await using var connection = context.Database.GetDbConnection();

        await connection.ExecuteAsync(sql, new
        {
            cap = player.TotalCapacity,
            level = player.Level,
            mana = player.Mana,
            manamax = player.MaxMana,
            health = player.HealthPoints,
            healthmax = player.MaxHealthPoints,
            Soul = player.SoulPoints,
            MaxSoul = player.MaxSoulPoints,
            player.Speed,
            player.StaminaMinutes,

            lookaddons = player.Outfit.Addon,
            lookbody = player.Outfit.Body,
            lookfeet = player.Outfit.Feet,
            lookhead = player.Outfit.Head,
            looklegs = player.Outfit.Legs,
            looktype = player.Outfit.LookType,
            posx = player.Location.X,
            posy = player.Location.Y,
            posz = player.Location.Z,

            skill_fist = player.GetSkillLevel(SkillType.Fist),
            skill_fist_tries = player.GetSkillTries(SkillType.Fist),
            skill_club = player.GetSkillLevel(SkillType.Club),
            skill_club_tries = player.GetSkillTries(SkillType.Club),
            skill_sword = player.GetSkillLevel(SkillType.Sword),
            skill_sword_tries = player.GetSkillTries(SkillType.Sword),
            skill_axe = player.GetSkillLevel(SkillType.Axe),
            skill_axe_tries = player.GetSkillTries(SkillType.Axe),
            skill_dist = player.GetSkillLevel(SkillType.Distance),
            skill_dist_tries = player.GetSkillTries(SkillType.Distance),
            skill_shielding = player.GetSkillLevel(SkillType.Shielding),
            skill_shielding_tries = player.GetSkillTries(SkillType.Shielding),
            skill_fishing = player.GetSkillLevel(SkillType.Fishing),
            skill_fishing_tries = player.GetSkillTries(SkillType.Fishing),
            MagicLevel = player.GetSkillLevel(SkillType.Magic),
            MagicLevelTries = player.GetSkillTries(SkillType.Magic),
            player.Experience,
            player.ChaseMode,
            player.FightMode,
            remaining_recovery_seconds =
                player.Conditions.TryGetValue(ConditionType.Regeneration, out var condition)
                    ? condition.RemainingTime / TimeSpan.TicksPerMillisecond
                    : 0,
            vocation = player.VocationType,
            playerId = player.Id
        }, commandTimeout: 5);
    }

    public async Task UpdatePlayers(IEnumerable<IPlayer> players)
    {
        var tasks = new List<Task>();

        foreach (var player in players)
        {
            tasks.Add(UpdatePlayer(player));
            if (UpdatePlayerInventory(player) is { } updates) tasks.AddRange(updates);
            tasks.Add(SaveBackpack(player));
        }

        await Task.WhenAll(tasks);
    }

    private List<Task> UpdatePlayerInventory(IPlayer player)
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
                await using var context = NewDbContext;
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
    
    public async Task SavePlayerInventory(IPlayer player)
    {
        await AddMissingInventoryRecords(player);

        if (UpdatePlayerInventory(player) is { } updates) await Task.WhenAll(updates);
    }
    
    private async Task AddMissingInventoryRecords(IPlayer player)
    {
        await using var context = NewDbContext;

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
    
    public async Task UpdatePlayerOnlineStatus(uint playerId, bool status)
    {
        await using var context = NewDbContext;

        var player = await context.Players.SingleOrDefaultAsync(x => x.PlayerId == playerId);
        if (player is null) return;

        player.Online = status;

        await context.SaveChangesAsync();
    }
}