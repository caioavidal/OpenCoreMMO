using System.Collections.Immutable;
using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Combat.Calculations;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Bases;

namespace NeoServer.Game.Items.Items.Weapons
{
    public class DistanceWeapon :Equipment, IDistanceWeaponItem
    {
        public DistanceWeapon(IItemType type, Location location) : base(type, location)
        {
        }

        public ImmutableHashSet<VocationType> AllowedVocations { get; }

        public byte ExtraAttack => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Attack);
        public byte ExtraHitChance => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.HitChance);
        public byte Range => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Range);

        public bool Use(ICombatActor actor, ICombatActor enemy, out CombatAttackType combatType)
        {
            var result = false;
            combatType = new CombatAttackType();

            if (actor is not IPlayer player) return false;

            if (player?.Inventory[Slot.Ammo] is not IAmmoItem ammo) return false;

            if (ammo.AmmoType != Metadata.AmmoType) return false;

            if (ammo.Amount <= 0) return false;

            var distance = (byte) actor.Location.GetSqmDistance(enemy.Location);

            var hitChance =
                (byte) (DistanceHitChanceCalculation.CalculateFor2Hands(player.GetSkillLevel(player.SkillInUse),
                    distance) + ExtraHitChance);
            combatType.ShootType = ammo.ShootType;

            var missed = DistanceCombatAttack.MissedAttack(hitChance);

            if (missed)
            {
                combatType.Missed = true;
                ammo.Throw();
                return true;
            }

            var maxDamage = player.CalculateAttackPower(0.09f, (ushort) (ammo.Attack + ExtraAttack));

            var combat = new CombatAttackValue(actor.MinimumAttackPower, maxDamage, Range, DamageType.Physical);

            if (DistanceCombatAttack.CalculateAttack(actor, enemy, combat, out var damage))
            {
                enemy.ReceiveAttack(actor, damage);
                result = true;
            }

            UseElementalDamage(actor, enemy, ref combatType, ref result, player, ammo, ref maxDamage, ref combat);

            if (result) ammo.Throw();

            return result;
        }

        public static bool IsApplicable(IItemType type)
        {
            return type.Attributes.GetAttribute(ItemAttribute.WeaponType) == "distance" &&
                   !type.HasFlag(ItemFlag.Stackable);
        }

        private void UseElementalDamage(ICombatActor actor, ICombatActor enemy, ref CombatAttackType combatType,
            ref bool result, IPlayer player, IAmmoItem ammo, ref ushort maxDamage, ref CombatAttackValue combat)
        {
            if (ammo.HasElementalDamage)
            {
                maxDamage = player.CalculateAttackPower(0.09f, (ushort) (ammo.ElementalDamage.Item2 + ExtraAttack));
                combat = new CombatAttackValue(actor.MinimumAttackPower, maxDamage, Range, ammo.ElementalDamage.Item1);
                if (ammo.ElementalDamage != null &&
                    DistanceCombatAttack.CalculateAttack(actor, enemy, combat, out var elementalDamage))
                {
                    combatType.DamageType = ammo.ElementalDamage.Item1;

                    enemy.ReceiveAttack(actor, elementalDamage);
                    result = true;
                }
            }
        }
    }
}