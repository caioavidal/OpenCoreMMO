using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Creatures.Enums;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Location;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Game.Creatures.Model.Monsters
{
    public class Monster : Creature, IMonster
    {

        public Monster(IMonsterType type) : base(type)
        {
            Metadata = type;
        }
        public Monster(IMonsterType type, ISpawnPoint spawn) : base(type)
        {
            Metadata = type;
            Spawn = spawn;
            Damages = new ConcurrentDictionary<ICreature, ushort>();

            OnDamaged += (enemy, victim, damage) => RecordDamage(enemy, damage);
            OnKilled += (enemy) => GiveExperience();
        }


        public MonsterState State { get; private set; } = MonsterState.Sleeping;

        public ConcurrentDictionary<ICreature, ushort> Damages;

        public void RecordDamage(ICreature enemy, ushort damage) => Damages.AddOrUpdate(enemy, damage, (key, oldValue) => (ushort)(oldValue + damage));

        private void GiveExperience()
        {
            var totalDamage = Damages.Sum(x => x.Value);

            foreach (var enemyDamage in Damages)
            {
                var damage = enemyDamage.Value;

                var damagePercent = damage * 100 / totalDamage;

                var exp = damagePercent * Experience / 100;

                enemyDamage.Key.GainExperience((uint)exp);
            }
        }

        public event Born OnWasBorn;

        public void Reborn()
        {
            ResetHealthPoints();
            SetNewLocation(Spawn.Location);
            OnWasBorn?.Invoke(this, Spawn.Location);
        }

        public override int ShieldDefend(int attack)
        {

            attack -= RandomDamagePower((Defense / 2), Defense);

            return attack;
        }

        public override int ArmorDefend(int attack)
        {
            if (ArmorRating > 3)
            {
                attack -= RandomDamagePower(ArmorRating / 2, ArmorRating - (ArmorRating % 2 + 1));
            }
            else if (ArmorRating > 0)
            {
                --attack;
            }
            return attack;
        }

        public new ushort Speed => Metadata.Speed;

        public override ushort AttackPower
        {
            get
            {
                if (Metadata.Attacks.TryGetValue(Game.Enums.Item.DamageType.Melee, out ICombatAttack combatAttack))
                {
                    return (ushort)(Math.Ceiling((combatAttack.Skill * (combatAttack.Attack * 0.05)) + (combatAttack.Attack * 0.5)));
                }

                return 0;
            }
        }


        public override ushort ArmorRating => Metadata.Armor;

        public override byte AutoAttackRange => 0;

        public IMonsterType Metadata { get; }
        public override IOutfit Outfit { get; protected set; }

        public override ushort MinimumAttackPower => 0;

        public override bool UsingDistanceWeapon => false;

        public ISpawnPoint Spawn { get; }

        public override ushort DefensePower => 30;

        public ushort Defense => Metadata.Defence;

        public uint Experience => Metadata.Experience;

        public void SetState(MonsterState state) => State = state;

        public override void SetAttackTarget(uint targetId)
        {
            if (targetId != 0)
            {
                SetState(MonsterState.Alive);
            }

            FollowCreature = true;

            base.SetAttackTarget(targetId);
        }
    }
}
