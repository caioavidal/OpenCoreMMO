using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using System;

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
            if (WasDamagedOnLastAttack)
            {
                attack -= RandomDamagePower((Defense / 2), Defense);
            }
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
    }
}
