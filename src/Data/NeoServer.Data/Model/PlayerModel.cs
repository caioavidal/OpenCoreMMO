using System.Collections.Generic;
using NeoServer.Game.Common.Creatures.Players;

namespace NeoServer.Data.Model
{
    public class PlayerModel
    {
        public PlayerModel()
        {
            PlayerInventoryItems = new HashSet<PlayerInventoryItemModel>();
            PlayerDepotItems = new HashSet<PlayerDepotItemModel>();
            PlayerItems = new HashSet<PlayerItemModel>();
        }

        public int PlayerId { get; set; }
        public int AccountId { get; set; }
        public string Name { get; set; }
        public int PlayerType { get; set; } //1 = player
        public uint Capacity { get; set; }
        public ushort Level { get; set; }
        public ushort Mana { get; set; }
        public ushort MaxMana { get; set; }
        public uint Health { get; set; }
        public uint MaxHealth { get; set; }
        public byte Soul { get; set; }
        public byte MaxSoul { get; set; }
        public ushort Speed { get; set; }
        public ushort StaminaMinutes { get; set; }
        public bool Online { get; set; }

        public int LookAddons { get; set; }
        public int LookBody { get; set; }
        public int LookFeet { get; set; }
        public int LookHead { get; set; }
        public int LookLegs { get; set; }
        public int LookType { get; set; }

        public int PosX { get; set; }
        public int PosY { get; set; }
        public int PosZ { get; set; }

        public int OfflineTrainingTime { get; set; }
        public int OfflineTrainingSkill { get; set; }

        public int SkillFist { get; set; }
        public double SkillFistTries { get; set; }

        public int SkillClub { get; set; }
        public double SkillClubTries { get; set; }

        public int SkillSword { get; set; }
        public double SkillSwordTries { get; set; }

        public int SkillAxe { get; set; }
        public double SkillAxeTries { get; set; }

        public int SkillDist { get; set; }
        public double SkillDistTries { get; set; }

        public int SkillShielding { get; set; }
        public double SkillShieldingTries { get; set; }

        public int SkillFishing { get; set; }
        public double SkillFishingTries { get; set; }

        public int MagicLevel { get; set; }
        public double MagicLevelTries { get; set; }
        public double Experience { get; set; }

        public ChaseMode ChaseMode { get; set; }
        public FightMode FightMode { get; set; }
        public Gender Gender { get; set; }
        public byte Vocation { get; set; }

        public AccountModel Account { get; set; }

        public virtual ICollection<PlayerItemModel> PlayerItems { get; set; }
        public virtual ICollection<PlayerDepotItemModel> PlayerDepotItems { get; set; }
        public virtual ICollection<PlayerInventoryItemModel> PlayerInventoryItems { get; set; }
        public virtual GuildMembershipModel GuildMember { get; set; }

        public bool IsMounted()
        {
            return false;
        }
    }
}