using NeoServer.Game.Contracts.World;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Location.Structs;

namespace NeoServer.Game.Contracts.Creatures
{
    public delegate void Born(IMonster monster, Location location);
    public interface IMonster : ICreature
    {
        event Born OnWasBorn;

        void Reborn();

        IMonsterType Metadata { get; }

        public ISpawnPoint Spawn { get; }
        public bool FromSpawn => Spawn != null;

        ushort Defense { get; }
        MonsterState State { get; }

        void SetState(MonsterState attacking);
    }
}
