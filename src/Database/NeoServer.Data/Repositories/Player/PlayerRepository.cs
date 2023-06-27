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
using NeoServer.Game.Common.Creatures;
using Serilog;

namespace NeoServer.Data.Repositories.Player;

public class PlayerRepository : BaseRepository<PlayerModel>, IPlayerRepository
{
    private readonly InventoryManager _inventoryManager;

    #region constructors

    public PlayerRepository(DbContextOptions<NeoContext> contextOptions, ILogger logger) : base(contextOptions,
        logger)
    {
        _inventoryManager = new InventoryManager(this);
    }

    #endregion

    public async Task UpdateAllPlayersToOffline()
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
        await _inventoryManager.SaveBackpack(player);
    }

    public async Task<PlayerModel> GetPlayer(string playerName)
    {
        await using var context = NewDbContext;
        return await context.Players.FirstOrDefaultAsync(x => x.Name.Equals(playerName));
    }

    public async Task UpdatePlayer(IPlayer player)
    {
        await using var context = NewDbContext;

        if (!context.Database.IsRelational()) return;

        await using var connection = context.Database.GetDbConnection();

        await connection.ExecuteAsync(PlayerQueries.UPDATE_PLAYER_SQL, new
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

            if (_inventoryManager.UpdatePlayerInventory(player) is { } updates) tasks.AddRange(updates);

            tasks.Add(_inventoryManager.SaveBackpack(player));
        }

        await Task.WhenAll(tasks);
    }

    public async Task SavePlayerInventory(IPlayer player)
    {
        await _inventoryManager.SavePlayerInventory(player);
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