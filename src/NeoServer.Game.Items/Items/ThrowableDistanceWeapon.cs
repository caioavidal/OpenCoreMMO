using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Combat.Calculations;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Body;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Combat.Structs;
using NeoServer.Game.Enums.Item;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Enums.Players;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace NeoServer.Game.Items.Items
{
    public class ThrowableDistanceWeapon : CumulativeItem, IDistanceWeaponItem
    {
        public ThrowableDistanceWeapon(IItemType type, Location location, IDictionary<ItemAttribute, IConvertible> attributes) : base(type, location, attributes) { }
        public ThrowableDistanceWeapon(IItemType type, Location location, byte amount) : base(type, location, amount) { }

        public byte Attack => Metadata.Attributes.GetAttribute<byte>(Enums.ItemAttribute.Attack);
        public byte ExtraHitChance => Metadata.Attributes.GetAttribute<byte>(Enums.ItemAttribute.HitChance);
        public byte Range => Metadata.Attributes.GetAttribute<byte>(Enums.ItemAttribute.Range);
     
        public ImmutableHashSet<VocationType> AllowedVocations { get; }

        public static new bool IsApplicable(IItemType type) => type.Attributes.GetAttribute(Enums.ItemAttribute.WeaponType) == "distance" && type.HasFlag(Enums.ItemFlag.Stackable);

        public bool Use(ICombatActor actor, ICombatActor enemy, out CombatAttackType combatType)
        {
            var result = false;
            combatType = new CombatAttackType();

            if (!(actor is IPlayer player)) return false;

            var hitChance = (byte)(DistanceHitChanceCalculation.CalculateFor1Hand(player.Skills[player.SkillInUse].Level, Range) + ExtraHitChance);

            var maxDamage = actor.CalculateAttackPower(0.09f, Attack);

            combatType.ShootType = Metadata.ShootType;

            var combat = new CombatAttackValue(actor.MinimumAttackPower, maxDamage, Range, hitChance, DamageType.Physical);

            if (DistanceCombatAttack.CalculateAttack(actor, enemy, combat, out var damage))
            {
                enemy.ReceiveAttack(enemy, damage);
                result = true;
            }
            return result;
        }
    }
}
