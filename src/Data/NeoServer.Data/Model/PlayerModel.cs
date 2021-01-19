using NeoServer.Data.Model;
using NeoServer.Game.Common.Players;
using System.Collections.Generic;

namespace NeoServer.Server.Model.Players
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
        public ushort Health { get; set; }
        public ushort MaxHealth { get; set; }
        public byte Soul { get; set; }
        public byte MaxSoul { get; set; }
        public ushort Speed { get; set; }
        public ushort StaminaMinutes { get; set; }
        public byte AmmoAmount { get; set; }
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
        public int SkillFistTries { get; set; }

        public int SkillClub { get; set; }
        public int SkillClubTries { get; set; }

        public int SkillSword { get; set; }
        public int SkillSwordTries { get; set; }

        public int SkillAxe { get; set; }
        public int SkillAxeTries { get; set; }

        public int SkillDist { get; set; }
        public int SkillDistTries { get; set; }

        public int SkillShielding { get; set; }
        public int SkillShieldingTries { get; set; }

        public int SkillFishing { get; set; }
        public int SkillFishingTries { get; set; }

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
        
        public bool IsMounted()
        {
            return false;
        }
    }
}