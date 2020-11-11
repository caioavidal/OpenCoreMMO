using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Location.Structs;

namespace NeoServer.Game.Contracts.Creatures
{
    public delegate void Born(IMonster monster, Location location);
    public delegate void Defende(IMonster monster, ICombatDefense defense);
    public interface IMonster : ICombatActor
    {
        event Born OnWasBorn;
        event Defende OnDefende;

        void Reborn();

        IMonsterType Metadata { get; }

        public ISpawnPoint Spawn { get; }
        public bool FromSpawn => Spawn != null;

        ushort Defense { get; }
        MonsterState State { get; }

        void SetState(MonsterState attacking);

        /// <summary>
        /// Select a target to attack
        /// </summary>
        void SelectTargetToAttack();
        void AddToTargetList(ICombatActor creature);
        void RemoveFromTargetList(ICreature creature);
        ushort Defende();
        void MoveAroundEnemy();
        void UpdateLastTargetChange();

        /// <summary>
        /// Set creature as enemy. If monster can't see creature it will be forgotten
        /// </summary>
        /// <param name="creature"></param>
        void SetAsEnemy(ICombatActor creature);

        uint Experience { get; }
        bool HasAnyTarget { get; }
        bool CanReachAnyTarget { get; }
        bool IsInCombat { get; }
        bool Defending { get; }
    }

}
