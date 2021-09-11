using System.Collections.Immutable;
using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Parsers;
using NeoServer.Game.Items.Bases;

namespace NeoServer.Game.Items.Items.Weapons
{
    public class MagicWeapon : Equipment, IDistanceWeaponItem
    {
        public MagicWeapon(IItemType type, Location location) : base(type, location)
        {
        }

        public ImmutableHashSet<VocationType> AllowedVocations { get; }
        public ShootType ShootType => Metadata.ShootType;

        public DamageType DamageType => Metadata.Attributes.HasAttribute(ItemAttribute.Damage)
            ? Metadata.DamageType
            : ShootTypeParser.ToDamageType(ShootType);

        public ushort MaxHitChance => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.MaxHitChance);
        public ushort MinHitChance => (ushort) (MaxHitChance / 2);
        public ushort ManaComsumption => Metadata.Attributes?.GetAttribute<ushort>(ItemAttribute.ManaUse) ?? 0;
        public byte Range => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Range);
        public WeaponType WeaponType => WeaponType.Magical;

        public bool Use(ICombatActor actor, ICombatActor enemy, out CombatAttackType combatType)
        {
            combatType = new CombatAttackType(ShootType);

            if (actor is not IPlayer player) return false;
            if (!player.HasEnoughMana(ManaComsumption)) return false;

            var combat = new CombatAttackValue((ushort) (MaxHitChance / 2), MaxHitChance, Range, DamageType);

            if (DistanceCombatAttack.CalculateAttack(actor, enemy, combat, out var damage))
            {
                player.ConsumeMana(ManaComsumption);
                enemy.ReceiveAttack(actor, damage);
                return true;
            }

            return false;
        }

        public static bool IsApplicable(IItemType type)
        {
            return type.Attributes.GetAttribute(ItemAttribute.WeaponType) is string weaponName &&
                   (weaponName == "wand" || weaponName == "rod");
        }
    }
}