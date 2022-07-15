using System.Collections.Immutable;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Creatures;

namespace NeoServer.Game.Common.Contracts.Creatures;

public delegate void Born(IMonster monster, Location.Structs.Location location);

public delegate void MonsterChangeState(IMonster monster, MonsterState fromState, MonsterState toState);

public interface IMonster : IWalkableMonster, ICombatActor
{
    IMonsterType Metadata { get; }

    public ISpawnPoint Spawn { get; }
    public bool FromSpawn => Spawn != null;

    ushort Defense { get; }
    MonsterState State { get; }

    /// <summary>
    ///     Experience that monster can give
    /// </summary>
    uint Experience { get; }

    bool CanReachAnyTarget { get; }

    /// <summary>
    ///     Returns true when monster is in combat
    /// </summary>
    bool IsInCombat { get; }

    bool Defending { get; }

    /// <summary>
    ///     Checks if monster is sleeping
    /// </summary>
    bool IsSleeping { get; }

    bool IsSummon { get; }
    ImmutableDictionary<ICreature, ushort> Damages { get; }
    bool IsHostile { get; }
    event Born OnWasBorn;
    event MonsterChangeState OnChangedState;

    void Reborn();

    /// <summary>
    ///     Select a target to attack
    /// </summary>
    void SelectTargetToAttack();

    /// <summary>
    ///     Executes defense action
    /// </summary>
    /// <returns>interval</returns>
    ushort Defend();

    void MoveAroundEnemy();
    void UpdateLastTargetChance();

    void Sleep();

    /// <summary>
    ///     Monster yells a sentence
    /// </summary>
    void Yell();

    /// <summary>
    ///     Changes monster's state based on targets and condition
    /// </summary>
    void ChangeState();

    void Escape();
    void Born(Location.Structs.Location location);
    void Summon(ISummonService summonService);
    void OnEnemyAppears(ICombatActor enemy);
}