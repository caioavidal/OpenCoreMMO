using NeoServer.Data.Model;
using NeoServer.Game.Contracts;
using NeoServer.Server.Contracts.Network;
using System.Linq;
using System.Threading.Tasks;
using NeoServer.Server.Model.Players;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures;
using NeoServer.Game.Creature.Model;
using System.Collections.Generic;
using NeoServer.Game.Common.Players;
using NeoServer.Game.Contracts.Items.Types;
using System;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Server.Commands
{
    public class PlayerLogInCommand : Command
    {
        private readonly AccountModel account;
        private readonly string characterName;
        private readonly Game game;
        private readonly IConnection connection;
        private CreaturePathAccess _creaturePathAccess;

        public PlayerLogInCommand(AccountModel account, string characterName, Game game, IConnection connection, CreaturePathAccess creaturePathAccess)
        {
            this.account = account;
            this.characterName = characterName;

            this.game = game;
            this.connection = connection;
            _creaturePathAccess = creaturePathAccess;
        }

        public override void Execute()
        {
            var playerRecord = account.Players.FirstOrDefault(p => p.Name.Equals(characterName));

            if (playerRecord == null)
            {
                //todo validations here
                return;
            }

            //MOVE TO CORRECT LOCAL AND CREATE LOADER

            var location = new Location((ushort)playerRecord.PosX, (ushort)playerRecord.PosY, (byte)playerRecord.PosZ);

            var outfit = new Outfit
            {
                LookType = 75
            };

            var iventory = new Dictionary<Slot, Tuple<IPickupable, ushort>>();

            var skills = new Dictionary<SkillType, ISkill>();

            skills.Add(SkillType.Axe, new Skill(SkillType.Axe, 1, (ushort)playerRecord.SkillAxe));
            skills.Add(SkillType.Club, new Skill(SkillType.Club, 1, (ushort)playerRecord.SkillClub));
            skills.Add(SkillType.Distance, new Skill(SkillType.Distance, 1, (ushort)playerRecord.SkillDist));
            skills.Add(SkillType.Fishing, new Skill(SkillType.Fishing, 1, (ushort)playerRecord.SkillFishing));
            skills.Add(SkillType.Fist, new Skill(SkillType.Fist, 1, (ushort)playerRecord.SkillFist));
            skills.Add(SkillType.Level, new Skill(SkillType.Level, 1, (ushort)playerRecord.Level));
            skills.Add(SkillType.Magic, new Skill(SkillType.Magic, 1, (ushort)playerRecord.Mana));
            skills.Add(SkillType.Shielding, new Skill(SkillType.Shielding, 1, (ushort)playerRecord.SkillShielding));
            skills.Add(SkillType.Speed, new Skill(SkillType.Speed, 1, (ushort)playerRecord.Speed));
            skills.Add(SkillType.Sword, new Skill(SkillType.Sword, 1, (ushort)playerRecord.SkillSword));

            var newPlayer = new Model.Players.Player(
                (uint)playerRecord.PlayerId,
                playerRecord.Name,
                playerRecord.ChaseMode,
                playerRecord.Capacity,
                playerRecord.Health,
                playerRecord.MaxHealth,
                playerRecord.Vocation,
                playerRecord.Gender,
                playerRecord.Online,
                playerRecord.Mana,
                playerRecord.MaxMana,
                playerRecord.FightMode,
                playerRecord.Soul,
                playerRecord.MaxSoul,
                skills, //ConvertToSkills(player),
                playerRecord.StaminaMinutes,
                outfit, //player.Outfit,
                iventory, //ConvertToInventory(player),
                playerRecord.Speed,
                location,
                _creaturePathAccess
                );

            game.CreatureManager.AddPlayer(newPlayer, connection);
            //game.CreatureManager.AddPlayer(playerRecord, connection);
        }
    }
}