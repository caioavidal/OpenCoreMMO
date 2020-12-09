using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Contracts.Creatures
{
    public delegate void Born(IMonster monster, Location location);
    public delegate void Defende(IMonster monster, ICombatDefense defense);
    public delegate void MonsterChangeState(IMonster monster,MonsterState fromState, MonsterState toState);
    public interface IMonster : IWalkableMonster, ICombatActor
    {
        event Born OnWasBorn;
        event Defende OnDefende;
        event DropLoot OnDropLoot;
        event MonsterChangeState OnChangedState;

        void Reborn();

        IMonsterType Metadata { get; }

        public ISpawnPoint Spawn { get; }
        public bool FromSpawn => Spawn != null;

        ushort Defense { get; }
        MonsterState State { get; }
        /// <summary>
        /// Select a target to attack
        /// </summary>
        void SelectTargetToAttack();
        void AddToTargetList(ICombatActor creature);
        void RemoveFromTargetList(ICreature creature);

        /// <summary>
        /// Executes defense action
        /// </summary>
        /// <returns>interval</returns>
        ushort Defend();
        void MoveAroundEnemy();
        void UpdateLastTargetChance();

        /// <summary>
        /// Set creature as enemy. If monster can't see creature it will be forgotten
        /// </summary>
        /// <param name="creature"></param>
        void SetAsEnemy(ICombatActor creature);
        void Sleep();

        /// <summary>
        /// Monster yells a sentence
        /// </summary>
        void Yell();

        /// <summary>
        /// Changes monster's state based on targets and condition
        /// </summary>
        void ChangeState();
        void Escape();

        /// <summary>
        /// Experience that monster can give
        /// </summary>
        uint Experience { get; }
        bool CanReachAnyTarget { get; }
        /// <summary>
        /// Returns true when monster is in combat
        /// </summary>
        bool IsInCombat { get; }
        bool Defending { get; }

        /// <summary>
        /// Checks if monster is sleeping
        /// </summary>
        bool IsSleeping { get; }
        bool IsSummon { get; }
    }

}
