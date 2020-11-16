using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Creatures.Combat.Attacks;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Combat.Structs;
using NeoServer.Game.Enums.Item;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Enums.Players;
using System;
using System.Collections.Immutable;

namespace NeoServer.Game.Items.Items
{
    public class MeleeWeapon : MoveableItem, IWeaponItem
    {
        public MeleeWeapon(IItemType itemType, Location location) : base(itemType, location)
        {
            //AllowedVocations  todo
        }

        public ushort Attack => Metadata.Attributes.GetAttribute<ushort>(ItemAttribute.Attack);

        public byte Defense => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Defense);

        public Tuple<DamageType, byte> ElementalDamage => Metadata.Attributes.GetWeaponElementDamage();

        public sbyte ExtraDefense => Metadata.Attributes.GetAttribute<sbyte>(ItemAttribute.ExtraDefense);

        public ImmutableHashSet<VocationType> AllowedVocations { get; }

        public bool Use(ICombatActor actor, ICombatActor enemy, out CombatAttackType combatType)
        {
            combatType = new CombatAttackType(DamageType.Melee);
            
            var result = false;

            if (Attack > 0)
            {
                var combat = new CombatAttackValue(actor.MinimumAttackPower, actor.CalculateAttackPower(0.085f, Attack), DamageType.Melee);
                if (MeleeCombatAttack.CalculateAttack(actor, enemy, combat, out var damage))
                {
                    enemy.ReceiveAttack(enemy, damage);
                    result = true;
                }
            }

            if (ElementalDamage != null)
            {
                var combat = new CombatAttackValue(actor.MinimumAttackPower, actor.CalculateAttackPower(0.085f, ElementalDamage.Item2), ElementalDamage.Item1);

                if (MeleeCombatAttack.CalculateAttack(actor, enemy, combat, out var damage))
                {
                    enemy.ReceiveAttack(enemy, damage);
                    result = true;
                }
            }

            return result;
        }

        public static bool IsApplicable(IItemType type) =>
            type.WeaponType switch
            {
                WeaponType.Axe => true,
                WeaponType.Club => true,
                WeaponType.Sword => true,
                _ => false
            };

    }
}
