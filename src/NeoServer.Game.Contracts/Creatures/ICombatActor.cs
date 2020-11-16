using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Spells;
using NeoServer.Game.Creatures.Enums;
using NeoServer.Game.Enums.Combat;
using NeoServer.Game.Enums.Combat.Structs;
using NeoServer.Server.Model.Players.Contracts;
using System;

namespace NeoServer.Game.Contracts.Creatures
{
    public delegate void OnAttackTargetChange(ICombatActor actor, uint oldTargetId, uint newTargetId);
    public delegate void Damage(ICombatActor enemy, ICombatActor victim, CombatDamage damage);
    public delegate void StopAttack(ICombatActor actor);
    public delegate void BlockAttack(ICombatActor creature, BlockType block);
    public delegate void Attack(ICombatActor creature, ICombatActor victim, CombatAttackType combat);
    public delegate void UseSpell(ICreature creature, ISpell spell);
    public delegate void ChangeVisibility(ICombatActor actor);

    public interface ICombatActor: IWalkableCreature
    {
        event Attack OnAttackEnemy;
        event BlockAttack OnBlockedAttack;
        event Damage OnDamaged;
        event Heal OnHeal;
        event Die OnKilled;
        event StopAttack OnStoppedAttack;
        event OnAttackTargetChange OnTargetChanged;
        event ChangeVisibility OnChangedVisibility;

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
        //bool Attack(ICombatActor enemy, ICombatAttack combatAttack);
        void Heal(ushort increasing);
        CombatDamage ReduceDamage(CombatDamage damage);
        void SetAttackTarget(uint targetId);
        int ShieldDefend(int attack);
        void StopAttack();
        void ResetHealthPoints();
        void TurnInvisible();
        void TurnVisible();
        void StartSpellCooldown(ISpell spell);
        bool SpellCooldownHasExpired(ISpell spell);
        bool CooldownHasExpired(CooldownType type);
        void ReceiveAttack(ICombatActor enemy, CombatDamage damage);
        bool Attack(ICombatActor enemy);
    }
}
