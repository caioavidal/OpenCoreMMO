using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.World;

namespace NeoServer.Game.Contracts.Creatures
{
    public delegate void Born(IMonster monster, Location location);
    public delegate void MonsterChangeState(IMonster monster,MonsterState fromState, MonsterState toState);
    public interface IMonster : IWalkableMonster, ICombatActor
    {
        event Born OnWasBorn;
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
        void Born(Location location);

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
