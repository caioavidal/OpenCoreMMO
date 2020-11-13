using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Combat.Calculations;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Body;
using NeoServer.Game.Creatures.Combat.Attacks;
using NeoServer.Game.Enums.Combat.Structs;
using NeoServer.Game.Enums.Item;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Enums.Players;
using NeoServer.Server.Model.Players.Contracts;
using System.Collections.Immutable;

namespace NeoServer.Game.Items.Items
{
    public class DistanceWeapon : MoveableItem, IDistanceWeaponItem
    {
        public DistanceWeapon(IItemType type, Location location) : base(type, location)
        {
        }
        public ImmutableHashSet<VocationType> AllowedVocations { get; }

        public byte MaxAttackDistance => 6;

        public static bool IsApplicable(IItemType type) => type.Attributes.GetAttribute(Enums.ItemAttribute.WeaponType) == "distance" && !type.HasFlag(Enums.ItemFlag.Stackable);

        public bool Use(ICombatActor actor, ICombatActor enemy, out CombatAttackValue combat)
        {
            combat = new CombatAttackValue();

            if (!(actor is IPlayer player)) return false;

            if (player?.Inventory[Slot.Ammo] == null) return false;

            if (!(player?.Inventory[Slot.Ammo] is AmmoItem ammo)) return false;

            if(ammo.AmmoType != Metadata.AmmoType) return false;

            if (ammo.Amount <= 0) return false;

            var maxDamage = actor.CalculateAttackPower(0.09f, ammo.Attack);

            var hitChance = DistanceHitChanceCalculation.CalculateFor2Hands(player.Skills[player.SkillInUse].Level, MaxAttackDistance);

            combat = new CombatAttackValue(actor.MinimumAttackPower, maxDamage, DamageType.Physical, MaxAttackDistance, ammo.ShootType, hitChance);

            if (DistanceCombatAttack.Instance.TryAttack(actor, enemy, combat, out var damage))
            {
                enemy.ReceiveAttack(enemy, damage);
                return true;
            }

            return false;
        }
    }
}
