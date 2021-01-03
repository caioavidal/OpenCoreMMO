using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Players;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Server.Model.Players.Contracts;
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

        public Tuple<DamageType, byte> ElementalDamage => Metadata.Attributes.GetWeaponElementDamage();

        public sbyte ExtraDefense => Metadata.Attributes.GetAttribute<sbyte>(ItemAttribute.ExtraDefense);

        public ImmutableHashSet<VocationType> AllowedVocations { get; }

        public bool Use(ICombatActor actor, ICombatActor enemy, out CombatAttackType combatType)
        {
            combatType = new CombatAttackType(DamageType.Melee);

            if (!(actor is IPlayer player)) return false;

            var result = false;

            if (Attack > 0)
            {
                var combat = new CombatAttackValue(actor.MinimumAttackPower, player.CalculateAttackPower(0.085f, Attack), DamageType.Melee);
                if (MeleeCombatAttack.CalculateAttack(actor, enemy, combat, out var damage))
                {
                    enemy.ReceiveAttack(actor, damage);
                    result = true;
                }
            }

            if (ElementalDamage != null)
            {
                var combat = new CombatAttackValue(actor.MinimumAttackPower, player.CalculateAttackPower(0.085f, ElementalDamage.Item2), ElementalDamage.Item1);

                if (MeleeCombatAttack.CalculateAttack(actor, enemy, combat, out var damage))
                {
                    enemy.ReceiveAttack(actor, damage);
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
