using Dapper;
using Microsoft.EntityFrameworkCore;
using NeoServer.Data.Interfaces;
using NeoServer.Data.Model;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Model.Players;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NeoServer.Data.Repositories
{
    public class AccountRepository : BaseRepository<AccountModel, NeoContext>, IAccountRepository
    {
        #region constructors

        public AccountRepository(NeoContext context) : base(context) { }

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

        public Task UpdatePlayers(IEnumerable<IPlayer> players)
        {
            Task[] tasks = new Task[players.Count()];
            var i = 0;
            foreach (var player in players)
            {
                var sql = $@"UPDATE players
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
                               AmmoAmount = @AmmoAmount,
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

                using (var connection = Context.Database.GetDbConnection())
                {
                    tasks[i++] = connection.ExecuteAsync(sql, new 
                    {
                        cap = player.TotalCapacity,
                        level = player.Level,
                        mana = player.Mana,
                        manamax = player.MaxMana,
                        health = player.HealthPoints,
                        healthmax = player.MaxHealthPoints,
                        Soul =  player.SoulPoints,
                        MaxSoul = player.MaxSoulPoints,
                        Speed = player.Speed,
                        StaminaMinutes = player.StaminaMinutes,
                        AmmoAmount = player.Inventory[Game.Common.Players.Slot.Ammo]?.Amount ?? 0,

                        lookaddons = player.Outfit.Addon,
                        lookbody = player.Outfit.Body,
                        lookfeet = player.Outfit.Feet,
                        lookhead= player.Outfit.Head,
                        looklegs = player.Outfit.Legs,
                        looktype = player.Outfit.LookType,
                        posx = player.Location.X,
                        posy = player.Location.Y,
                        posz = player.Location.Z,

                        skill_fist = player.Skills[Game.Common.Creatures.SkillType.Fist].Level,
                        skill_fist_tries = player.Skills[Game.Common.Creatures.SkillType.Fist].Count,
                        skill_club = player.Skills[Game.Common.Creatures.SkillType.Club].Level,
                        skill_club_tries = player.Skills[Game.Common.Creatures.SkillType.Club].Count,
                        skill_sword = player.Skills[Game.Common.Creatures.SkillType.Sword].Level,
                        skill_sword_tries = player.Skills[Game.Common.Creatures.SkillType.Sword].Count,
                        skill_axe = player.Skills[Game.Common.Creatures.SkillType.Axe].Level,
                        skill_axe_tries = player.Skills[Game.Common.Creatures.SkillType.Axe].Count,
                        skill_dist = player.Skills[Game.Common.Creatures.SkillType.Distance].Level,
                        skill_dist_tries = player.Skills[Game.Common.Creatures.SkillType.Distance].Count,
                        skill_shielding = player.Skills[Game.Common.Creatures.SkillType.Shielding].Level,
                        skill_shielding_tries = player.Skills[Game.Common.Creatures.SkillType.Shielding].Count,
                        skill_fishing = player.Skills[Game.Common.Creatures.SkillType.Fishing].Level,
                        skill_fishing_tries = player.Skills[Game.Common.Creatures.SkillType.Fishing].Count,
                        MagicLevel = player.Skills[Game.Common.Creatures.SkillType.Magic].Level,
                        MagicLevelTries = player.Skills[Game.Common.Creatures.SkillType.Magic].Count,

                        Experience = player.Experience,
                        ChaseMode = player.ChaseMode,
                        FightMode = player.FightMode,

                        vocation = player.VocationType,
                        playerId = player.Id
                    }, commandTimeout: 5);
                }
            }

            return Task.WhenAll(tasks);
        }

        public async Task<PlayerModel> GetPlayer(string playerName)
        {
            return await Context.Players.FirstOrDefaultAsync(x => x.Name.Equals(playerName));
        }
        public async Task AddPlayerToVipList(int accountId, int playerId)
        {
            await Context.AccountsVipList.AddAsync(new AccountVipListModel()
            {
                AccountId = accountId,
                PlayerId = playerId,
            });

            await CommitChanges();
        }
        public async Task RemoveFromVipList(int accountId, int playerId)
        {
            var item = await Context.AccountsVipList.SingleOrDefaultAsync(x => x.PlayerId == playerId && x.AccountId == accountId);
            Context.AccountsVipList.Remove(item);
            await CommitChanges();
        }

        #endregion
    }
}
