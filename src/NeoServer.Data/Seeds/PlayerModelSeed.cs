using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoServer.Server.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Data.Seeds
{
    public class PlayerModelSeed
    {
        public static void Seed(EntityTypeBuilder<PlayerModel> builder)
        {
            builder.HasData
                (
                new PlayerModel
                {
                    PlayerId = 1,
                    PlayerType = 1,
                    AccountId = 1,
                    Name = "GOD",
                    ChaseMode = NeoServer.Game.Common.Players.ChaseMode.Follow,
                    Capacity = 90000,
                    Level = 800,
                    Health = 4440,
                    MaxHealth = 4440,
                    Vocation = 4,
                    Gender = NeoServer.Game.Common.Players.Gender.Male,
                    Speed = 800,
                    Online = false,
                    Mana = 1750,
                    MaxMana = 1750,
                    Soul = 100,
                    MaxSoul = 100,
                    PosX = 1020,
                    PosY = 1022,
                    PosZ = 7,
                    StaminaMinutes = 2520,
                    LookType = 75,
                    SkillAxe = 10,
                    SkillSword = 40,
                    SkillClub = 10,
                    SkillDist = 30,
                    SkillFishing = 10,
                    SkillFist = 10,
                    MagicLevel = 100,
                    Experience = 8469559800,
                    FightMode = Game.Common.Players.FightMode.Attack
                }
              );
        }
    }
}
