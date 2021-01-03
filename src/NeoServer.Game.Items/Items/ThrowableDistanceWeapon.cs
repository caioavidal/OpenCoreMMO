using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Combat.Calculations;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Players;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Body;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace NeoServer.Game.Items.Items
{
    public class ThrowableDistanceWeapon : Cumulative, IThrowableDistanceWeaponItem
    {
        public ThrowableDistanceWeapon(IItemType type, Location location, IDictionary<ItemAttribute, IConvertible> attributes) : base(type, location, attributes) { }
        public ThrowableDistanceWeapon(IItemType type, Location location, byte amount) : base(type, location, amount) { }

        public byte Attack => Metadata.Attributes.GetAttribute<byte>(Common.ItemAttribute.Attack);
        public byte ExtraHitChance => Metadata.Attributes.GetAttribute<byte>(Common.ItemAttribute.HitChance);
        public byte Range => Metadata.Attributes.GetAttribute<byte>(Common.ItemAttribute.Range);
     
        public ImmutableHashSet<VocationType> AllowedVocations { get; }

        public static new bool IsApplicable(IItemType type) => type.Attributes.GetAttribute(Common.ItemAttribute.WeaponType) == "distance" && type.HasFlag(Common.ItemFlag.Stackable);

        public bool Use(ICombatActor actor, ICombatActor enemy, out CombatAttackType combatType)
        {
            var result = false;
            combatType = new CombatAttackType(Metadata.ShootType);

            if (!(actor is IPlayer player)) return false;

            var hitChance = (byte)(DistanceHitChanceCalculation.CalculateFor1Hand(player.Skills[player.SkillInUse].Level, Range) + ExtraHitChance);
            var missed = DistanceCombatAttack.MissedAttack(hitChance);

            if (missed)
            {
                combatType.Missed = true;
                return true;
            }

            var maxDamage = player.CalculateAttackPower(0.09f, Attack);

            var combat = new CombatAttackValue(actor.MinimumAttackPower, maxDamage, Range, DamageType.Physical);

            if (DistanceCombatAttack.CalculateAttack(actor, enemy, combat, out var damage))
            {
                enemy.ReceiveAttack(actor, damage);
                result = true;
            }
            return result;
        }
    }
}
