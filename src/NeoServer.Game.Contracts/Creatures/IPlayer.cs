using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Players;
using NeoServer.Game.Common.Talks;
using System.Collections.Generic;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Common.Parsers;
using NeoServer.Game.Contracts;

namespace NeoServer.Server.Model.Players.Contracts
{
    public delegate void CancelWalk(IPlayer player);
    public delegate void ClosedContainer(IPlayer player, byte containerId, IContainer container);
    public delegate void OpenedContainer(IPlayer player, byte containerId, IContainer container);
    public delegate void ReduceMana(IPlayer player);
    public delegate void CannotUseSpell(IPlayer player, ISpell spell, InvalidOperation error);
    public delegate void PlayerLevelAdvance(IPlayer player, SkillType type, int fromLevel, int toLevel);
    public delegate void OperationFail(uint id, string message);
    public delegate void LookAt(IPlayer player, IThing thing, bool isClose);
    public delegate void PlayerGainSkillPoint(IPlayer player, SkillType type);
    public delegate void UseItem(IPlayer player, ICreature creature, IConsumable consumable);
    public interface IPlayer : ICombatActor
    {
        event UseSpell OnUsedSpell;

        ushort Level { get; }

        byte LevelPercent { get; }

        uint Experience { get; }
        byte SoulPoints { get; }

        float CarryStrength { get; }
        public string Guild { get; }
        IDictionary<SkillType, ISkill> Skills { get; }
        ushort StaminaMinutes { get; }

        FightMode FightMode { get; }
        ChaseMode ChaseMode { get; }
        byte SecureMode { get; }

        bool InFight { get; }
        IPlayerContainerList Containers { get; }

        event CancelWalk OnCancelledWalk;
        event CannotUseSpell OnCannotUseSpell;
        event OperationFail OnOperationFailed;
        event LookAt OnLookedAt;
        event PlayerGainSkillPoint OnGainedSkillPoint;
        event UseItem OnUsedItem;

        IInventory Inventory { get; }
        ushort Mana { get; }
        ushort MaxMana { get; }
        SkillType SkillInUse { get; }
        bool CannotLogout { get; }
        uint Id { get; }
        bool HasDepotOpened { get; }
        VocationType VocationType { get;  }

        //  IAction PendingAction { get; }

        /// <summary>
        /// Changes player outfit
        /// </summary>
        /// <param name="outfit"></param>
        void ChangeOutfit(IOutfit outfit);

        uint ChooseToRemoveFromKnownSet();

        /// <summary>
        /// Checks if player knows creature with given id
        /// </summary>
        /// <param name="creatureId"></param>
        /// <returns></returns>
        bool KnowsCreatureWithId(uint creatureId);

        /// <summary>
        /// Get skill info
        /// </summary>
        /// <param name="skillType"></param>
        /// <returns></returns>
        byte GetSkillInfo(SkillType skillType);

        /// <summary>
        /// Changes player's fight mode
        /// </summary>
        /// <param name="fightMode"></param>
        void SetFightMode(FightMode fightMode);
        /// <summary>
        /// Changes player's chase mode
        /// </summary>
        /// <param name="chaseMode"></param>
        void SetChaseMode(ChaseMode chaseMode);
        /// <summary>
        /// Toogle Secure Mode 
        /// </summary>
        /// <param name="secureMode"></param>
        void SetSecureMode(byte secureMode);
        byte GetSkillPercent(SkillType type);

        void AddKnownCreature(uint creatureId);
        /// <summary>
        /// Sets where player is turned to
        /// </summary>
        /// <param name="direction"></param>
        void SetDirection(Direction direction);

        void ResetIdleTime();
        void CancelWalk();
        bool CanMoveThing(Location location);
        void Say(string message, TalkType talkType);
        bool HasEnoughMana(ushort mana);
        void ConsumeMana(ushort mana);
        bool HasEnoughLevel(ushort level);
        bool Logout();
        ushort CalculateAttackPower(float attackRate, ushort attack);
        void LookAt(ITile tile);
        void LookAt(byte containerId, sbyte containerSlot);
        void LookAt(Slot slot);
        /// <summary>
        /// Health and mana recovery
        /// </summary>
        void Recover();
        void HealMana(ushort increasing);
        void Use(IConsumable item, ICreature creature);
        bool Feed(IFood food);
        Result MoveThing(IStore source, IStore destination, IThing thing, byte amount, byte fromPosition, byte? toPosition);

        string IThing.InspectionText => $"{Name} (Level {Level}). He is a {VocationTypeParser.Parse(VocationType).ToLower()}{GuildText}";
        private string GuildText => string.IsNullOrWhiteSpace(Guild) ? string.Empty : $". He is a member of {Guild}";

        uint TotalCapacity { get; }
        bool Recovering { get; }
    }
}
