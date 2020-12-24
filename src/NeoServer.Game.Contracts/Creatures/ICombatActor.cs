using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Spells;
using NeoServer.Game.Creatures.Enums;
using NeoServer.Game.Common.Combat;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Location.Structs;
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
    public delegate void OnPropagateAttack(ICombatActor actor, CombatDamage damage, Coordinate[] area);
    public delegate void DropLoot(ICombatActor actor, ILoot loot);
    public interface ICombatActor : IWalkableCreature
    {
        event Attack OnAttackEnemy;
        event BlockAttack OnBlockedAttack;
        event Damage OnDamaged;
        event Heal OnHeal;
        event Die OnKilled;
        event StopAttack OnStoppedAttack;
        event OnAttackTargetChange OnTargetChanged;
        event ChangeVisibility OnChangedVisibility;
        event OnPropagateAttack OnPropagateAttack;

        ushort ArmorRating { get; }
        bool Attacking { get; }
        uint AutoAttackTargetId { get; }
        decimal BaseAttackSpeed { get; }
        decimal BaseDefenseSpeed { get; }

        bool InFight { get; }
        bool IsDead { get; }
        ushort MinimumAttackPower { get; }
        bool UsingDistanceWeapon { get; }
        uint AttackEvent { get; set; }

        int ArmorDefend(int attack);
        //bool Attack(ICombatActor enemy, ICombatAttack combatAttack);
        void Heal(ushort increasing);
        CombatDamage ReduceDamage(CombatDamage damage);
        void SetAttackTarget(ICreature target);
        int ShieldDefend(int attack);
        void StopAttack();
        void ResetHealthPoints();
        void TurnInvisible();
        void TurnVisible();
        void StartSpellCooldown(ISpell spell);
        bool SpellCooldownHasExpired(ISpell spell);
        bool CooldownHasExpired(CooldownType type);

        /// <summary>
        /// Creature receive attack damage from enemy
        /// </summary>
        /// <param name="enemy"></param>
        /// <param name="damage"></param>
        /// <returns>Returns true when damage was bigger than 0</returns>
        bool ReceiveAttack(ICombatActor enemy, CombatDamage damage);
        bool Attack(ICreature creature);
        void SetAsInFight();
        void PropagateAttack(Coordinate[] area, CombatDamage damage);
    }
}
