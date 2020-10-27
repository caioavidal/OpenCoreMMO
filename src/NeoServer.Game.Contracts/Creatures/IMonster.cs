using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Location.Structs;

namespace NeoServer.Game.Contracts.Creatures
{
    public delegate void Born(IMonster monster, Location location);
    public delegate void Defende(IMonster monster, ICombatDefense defense);
    public interface IMonster : ICreature
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
        void SelectTargetToAttack();
        void AddToTargetList(ICreature creature);
        void RemoveFromTargetList(ICreature creature);
        ushort Defende();

        uint Experience { get; }
        bool HasAnyTarget { get; }
        bool CanReachAnyTarget { get; }
        bool IsInCombat { get; }
        bool Defending { get; }
    }

}
