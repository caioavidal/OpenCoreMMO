using System;
using System.Collections.Generic;
using System.Text;
using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Combat.Calculations;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Parsers;
using NeoServer.Game.Items.Bases;

namespace NeoServer.Game.Items.Items.Weapons
{
    public class ThrowableDistanceWeapon : CumulativeEquipment, IThrowableDistanceWeaponItem
    {
        public ThrowableDistanceWeapon(IItemType type, Location location,
            IDictionary<ItemAttribute, IConvertible> attributes) : base(type, location,attributes)
        {
        }

        public ThrowableDistanceWeapon(IItemType type, Location location, byte amount) : base(type, location,amount)
        {
        }
        
        public override bool CanBeDressed(IPlayer player)
        {
            if (Guard.IsNullOrEmpty(Vocations)) return true;
            
            foreach (var vocation in Vocations)
            {
                if (vocation == player.VocationType) return true;
            }

            return false;
        }

        private byte ExtraHitChance => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.HitChance);
        public byte Attack => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Attack);
        public byte Range => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Range);
        private byte Defense => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Defense);
        private Tuple<DamageType, byte> ElementalDamage => Metadata.Attributes.GetWeaponElementDamage();

        public bool Use(ICombatActor actor, ICombatActor enemy, out CombatAttackType combatType)
        {
            combatType = new CombatAttackType(Metadata.ShootType);

            if (actor is not IPlayer player) return false;

            var hitChance =
                (byte)(DistanceHitChanceCalculation.CalculateFor1Hand(player.GetSkillLevel(player.SkillInUse), Range) +
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

        protected override string PartialInspectionText
        {
            get
            {
                var range = Range > 0 ? $"Range: {Range}" : string.Empty;
                var hit = ExtraHitChance > 0 ? $"Hit% +{ExtraHitChance}" : string.Empty;
                var elementalDamageText = ElementalDamage is not null && ElementalDamage.Item2 > 0
                    ? $" + {ElementalDamage.Item2} {DamageTypeParser.Parse(ElementalDamage.Item1)},"
                    : ",";

                var stringBuilder = new StringBuilder();

                if (!string.IsNullOrWhiteSpace(range)) stringBuilder.Append($"{range}, ");

                stringBuilder.Append($"Atk: {Attack}{elementalDamageText} ");
                stringBuilder.Append($"Def: {Defense}, ");

                if (!string.IsNullOrWhiteSpace(hit)) stringBuilder.Append($"{hit}, ");

                stringBuilder.Remove(stringBuilder.Length - 2, 2);

                return stringBuilder.ToString();
            }
        }

        public static bool IsApplicable(IItemType type) =>
            type.Attributes.GetAttribute(ItemAttribute.WeaponType) == "distance" &&
            type.HasFlag(ItemFlag.Stackable);
    }
}