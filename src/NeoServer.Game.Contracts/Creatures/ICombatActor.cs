using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Creatures.Enums;
using NeoServer.Game.Enums.Combat;
using System;

namespace NeoServer.Game.Contracts.Creatures
{
    public delegate void OnAttackTargetChange(uint oldTargetId, uint newTargetId);
    public delegate void Damage(ICreature enemy, ICreature victim, ICombatAttack attack, ushort healthDamage);
    public delegate void StopAttack(ICreature actor);
    public delegate void BlockAttack(ICreature creature, BlockType block);
    public delegate void Attack(ICreature creature, ICreature victim, ICombatAttack combatAttack);
    public interface ICombatActor : INeedsCooldowns
    {
        event OnAttackTargetChange OnTargetChanged;
        event Damage OnDamaged;
        event StopAttack OnStoppedAttack;
        event BlockAttack OnBlockedAttack;

        /// <summary>
        /// Gets the id of the actor.
        /// </summary>
        uint ActorId { get; }

        /// <summary>
        /// Gets the blood type of the actor.
        /// </summary>
        BloodType Blood { get; }

        uint AutoAttackTargetId { get; }

        byte AutoAttackRange { get; }

        byte AutoAttackCredits { get; }

        byte AutoDefenseCredits { get; }

        DateTime LastAttackTime { get; }

        TimeSpan LastAttackCost { get; }

        TimeSpan CombatCooldownTimeRemaining { get; }

        /// <summary>
        /// Gets a metric of how fast an Actor can earn a new AutoAttack credit per second.
        /// </summary>
        decimal BaseAttackSpeed { get; }

        /// <summary>
        /// Gets a metric of how fast an Actor can earn a new AutoDefense credit per second.
        /// </summary>
        decimal BaseDefenseSpeed { get; }

        ushort AttackPower { get; }

        ushort DefensePower { get; }

        ushort ArmorRating { get; }
        uint LastCombatEvent { get; set; }
        ushort MinimumAttackPower { get; }
        bool Attacking { get; }

        void SetAttackTarget(uint targetId);

        void UpdateLastAttack(TimeSpan cost);

        void CheckAutoAttack(IThing thingChanged, IThingStateChangedEventArgs eventAgrs);
        void Attack(ICreature enemy);
        void StopAttack();
    }
}
