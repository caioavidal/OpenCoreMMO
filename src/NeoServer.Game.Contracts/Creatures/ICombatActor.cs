using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Spells;
using NeoServer.Game.Creatures.Enums;
using NeoServer.Game.Enums.Combat;
using System;

namespace NeoServer.Game.Contracts.Creatures
{
    public delegate void OnAttackTargetChange(ICombatActor actor, uint oldTargetId, uint newTargetId);
    public delegate void Damage(ICombatActor enemy, ICombatActor victim, ICombatAttack attack, ushort healthDamage);
    public delegate void StopAttack(ICombatActor actor);
    public delegate void BlockAttack(ICombatActor creature, BlockType block);
    public delegate void Attack(ICombatActor creature, ICombatActor victim, ICombatAttack combatAttack);
    public delegate void UseSpell(ICreature creature, ISpell spell);

    public interface ICombatActor: IWalkableCreature
    {
        event Attack OnAttack;
        event BlockAttack OnBlockedAttack;
        event Damage OnDamaged;
        event Heal OnHeal;
        event Die OnKilled;
        event StopAttack OnStoppedAttack;
        event OnAttackTargetChange OnTargetChanged;

        ushort ArmorRating { get; }
        bool Attacking { get; }
        ushort AttackPower { get; }
        byte AutoAttackRange { get; }
        uint AutoAttackTargetId { get; }
        decimal BaseAttackSpeed { get; }
        decimal BaseDefenseSpeed { get; }
        ushort DefensePower { get; }
      
        bool InFight { get; }
        bool IsDead { get; }
        ushort MinimumAttackPower { get; }
        bool UsingDistanceWeapon { get; }
        uint AttackEvent { get; set; }

        int ArmorDefend(int attack);
        bool Attack(ICombatActor enemy, ICombatAttack combatAttack);
        void Heal(ushort increasing);
        void ReceiveAttack(ICombatActor enemy, ICombatAttack attack);
        void ReceiveAttack(ICombatActor enemy, ICombatAttack attack, ushort damage);
        ushort ReduceDamage(int attack);
        void SetAttackTarget(uint targetId);
        int ShieldDefend(int attack);
        void StopAttack();
        void ResetHealthPoints();
    }
}
