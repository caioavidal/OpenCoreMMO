using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using NeoServer.Data.Interfaces;
using NeoServer.Data.Model;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Players;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Model.Players;

namespace NeoServer.Data.Repositories
{
    public class AccountRepository : BaseRepository<AccountModel, NeoContext>, IAccountRepository
    {
        #region constructors

        public AccountRepository(NeoContext context) : base(context)
        {
        }

        #endregion

        #region public methods implementation

        public IQueryable<AccountModel> GetAccount(string name, string password)
        {
            return Context.Accounts.Where(x => x.Name.Equals(name) && x.Password.Equals(password)).AsQueryable();
            //.Include(x => x.Players)
            //.ThenInclude(x => x.PlayerItems)

            //.Include(x => x.Players)
            //.ThenInclude(x => x.PlayerInventoryItems)
            //.Include(x => x.VipList)
            //.ThenInclude(x => x.Player)
            //.SingleOrDefaultAsync();
        }


        public async Task<PlayerModel> GetPlayer(string accountName, string password, string charName)
        {
            return await Context.Players.Where(x => x.Account.Name.Equals(accountName) &&
                                                    x.Account.Password.Equals(password) &&
                                                    x.Name.Equals(charName))
                .Include(x => x.PlayerItems)
                .Include(x => x.PlayerInventoryItems)
                .Include(x => x.Account)
                .ThenInclude(x => x.VipList)
                .ThenInclude(x => x.Player)
                .Include(x => x.GuildMember)
                .ThenInclude(x => x.Guild).SingleOrDefaultAsync();
        }


        public Task UpdatePlayer(IPlayer player, DbConnection conn = null)
        {
            var sql = @"UPDATE players
                           SET --id = @id,
                               --account_id = @account_id,
                               --name = @name,
                               --player_type = @player_type,
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
                               --Online = @Online,
                               lookaddons = @lookaddons,
                               lookbody = @lookbody,
                               lookfeet = @lookfeet,
                               lookhead = @lookhead,
                               looklegs = @looklegs,
                               looktype = @looktype,
                               posx = @posx,
                               posy = @posy,
                               posz = @posz,
                               --offlinetraining_time = @offlinetraining_time,
                               --offlinetraining_skill = @offlinetraining_skill,
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
                               --sex = @sex,
                               vocation = @vocation
                         WHERE id = @playerId";

            Func<DbConnection, Task> executeQuery = connection => connection.ExecuteAsync(sql, new
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

                skill_fist = player.Skills[SkillType.Fist].Level,
                skill_fist_tries = player.Skills[SkillType.Fist].Count,
                skill_club = player.Skills[SkillType.Club].Level,
                skill_club_tries = player.Skills[SkillType.Club].Count,
                skill_sword = player.Skills[SkillType.Sword].Level,
                skill_sword_tries = player.Skills[SkillType.Sword].Count,
                skill_axe = player.Skills[SkillType.Axe].Level,
                skill_axe_tries = player.Skills[SkillType.Axe].Count,
                skill_dist = player.Skills[SkillType.Distance].Level,
                skill_dist_tries = player.Skills[SkillType.Distance].Count,
                skill_shielding = player.Skills[SkillType.Shielding].Level,
                skill_shielding_tries = player.Skills[SkillType.Shielding].Count,
                skill_fishing = player.Skills[SkillType.Fishing].Level,
                skill_fishing_tries = player.Skills[SkillType.Fishing].Count,
                MagicLevel = player.Skills[SkillType.Magic].Level,
                MagicLevelTries = player.Skills[SkillType.Magic].Count,
                player.Experience,
                player.ChaseMode,
                player.FightMode,

                vocation = player.VocationType,
                playerId = player.Id
            }, commandTimeout: 5);


            if (conn is null)
                using (var connection = Context.Database.GetDbConnection())
                {
                    return executeQuery.Invoke(connection);
                }

            return executeQuery.Invoke(conn);
        }

        public async Task UpdatePlayers(IEnumerable<IPlayer> players)
        {
            if (!Context.Database.IsRelational()) return;

            var tasks = new List<Task>();

            using (var connection = Context.Database.GetDbConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    foreach (var player in players)
                    {
                        tasks.Add(UpdatePlayer(player, connection));
                        tasks.AddRange(UpdatePlayerInventory(player, connection));
                    }

                    await Task.WhenAll(tasks);
                    await transaction.CommitAsync();
                }
            }
        }

        public async Task<PlayerModel> GetPlayer(string playerName)
        {
            return await Context.Players.FirstOrDefaultAsync(x => x.Name.Equals(playerName));
        }

        public Task[] UpdatePlayerInventory(IPlayer player, DbConnection conn = null)
        {
            if (player is null) return Array.Empty<Task>();
            if (!Context.Database.IsRelational()) return Array.Empty<Task>();

            var sql = @"UPDATE player_inventory_items
                           SET sid = @sid,
                               count = @count
                         WHERE player_id = @playerId and slot_id = @pid";

            Func<DbConnection, Task[]> executeQueries = connection =>
            {
                var tasks = new List<Task>();

                foreach (var slot in new[]
                {
                    Slot.Necklace, Slot.Head, Slot.Backpack, Slot.Left, Slot.Body, Slot.Right, Slot.Ring, Slot.Legs,
                    Slot.Ammo, Slot.Feet
                })
                {
                    var item = player.Inventory[slot];
                    tasks.Add(connection.ExecuteAsync(sql, new
                    {
                        sid = item?.Metadata?.TypeId ?? 0,
                        count = item?.Amount ?? 0,
                        playerId = player.Id,
                        pid = (int) slot
                    }, commandTimeout: 5));
                }

                return tasks.ToArray();
            };

            if (conn is null)
                using (var connection = Context.Database.GetDbConnection())
                {
                    return executeQueries.Invoke(connection);
                }

            return executeQueries.Invoke(conn);
        }

        public async Task AddPlayerToVipList(int accountId, int playerId)
        {
            await Context.AccountsVipList.AddAsync(new AccountVipListModel
            {
                AccountId = accountId,
                PlayerId = playerId
            });

            await CommitChanges();
        }

        public async Task RemoveFromVipList(int accountId, int playerId)
        {
            var item = await Context.AccountsVipList.SingleOrDefaultAsync(x =>
                x.PlayerId == playerId && x.AccountId == accountId);
            Context.AccountsVipList.Remove(item);
            await CommitChanges();
        }

        #endregion
    }
}