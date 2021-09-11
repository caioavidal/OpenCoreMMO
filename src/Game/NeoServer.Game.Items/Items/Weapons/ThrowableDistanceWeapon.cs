using System;
using System.Collections.Generic;
using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Combat.Calculations;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Items.Weapons
{
    public class ThrowableDistanceWeapon : Cumulative, IThrowableDistanceWeaponItem
    {
        public ThrowableDistanceWeapon(IItemType type, Location location,
            IDictionary<ItemAttribute, IConvertible> attributes) : base(type, location, attributes)
        {
        }

        public ThrowableDistanceWeapon(IItemType type, Location location, byte amount) : base(type, location, amount)
        {
        }

        public byte ExtraHitChance => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.HitChance);

        public byte Attack => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Attack);
        public byte Range => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Range);

        public bool Use(ICombatActor actor, ICombatActor enemy, out CombatAttackType combatType)
        {
            combatType = new CombatAttackType(Metadata.ShootType);

            if (actor is not IPlayer player) return false;

            var hitChance =
                (byte) (DistanceHitChanceCalculation.CalculateFor1Hand(player.GetSkillLevel(player.SkillInUse), Range) +
                        ExtraHitChance);
            var missed = DistanceCombatAttack.MissedAttack(hitChance);

            if (missed)
            {
                combatType.Missed = true;
                return true;
            }

            var maxDamage = player.CalculateAttackPower(0.09f, Attack);

            var combat = new CombatAttackValue(actor.MinimumAttackPower, maxDamage, Range, DamageType.Physical);

            if (!DistanceCombatAttack.CalculateAttack(actor, enemy, combat, out var damage)) return false;

            enemy.ReceiveAttack(actor, damage);

            return true;
        }

        public static bool IsApplicable(IItemType type)
        {
            return type.Attributes.GetAttribute(ItemAttribute.WeaponType) == "distance" &&
                   type.HasFlag(ItemFlag.Stackable);
        }

        public void DressedIn(IPlayer player)
        {
            
        }

        public void UndressFrom(IPlayer player)
        {
        }
    }
}