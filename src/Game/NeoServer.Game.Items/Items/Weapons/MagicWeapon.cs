using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Parsers;
using NeoServer.Game.Common.Players;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Body;
using System.Collections.Immutable;

namespace NeoServer.Game.Items.Items
{

    public class MagicWeapon : MoveableItem, IDistanceWeaponItem
    {
        public MagicWeapon(IItemType type, Location location) : base(type, location)
        {
        }
        public ImmutableHashSet<VocationType> AllowedVocations { get; }
        public byte Range => Metadata.Attributes.GetAttribute<byte>(Common.ItemAttribute.Range);
        public ShootType ShootType => Metadata.ShootType;
        public DamageType DamageType => Metadata.Attributes.HasAttribute(Common.ItemAttribute.Damage) ? Metadata.DamageType : ShootTypeParser.ToDamageType(ShootType);
        public ushort MaxHitChance => Metadata.Attributes.GetAttribute<byte>(Common.ItemAttribute.MaxHitChance);
        public ushort MinHitChance => (ushort)(MaxHitChance / 2);
        public ushort ManaComsumption => Metadata.Attributes?.GetAttribute<ushort>(Common.ItemAttribute.ManaUse) ?? 0;
        public WeaponType WeaponType => WeaponType.Magical;

        public static bool IsApplicable(IItemType type) => type.Attributes.GetAttribute(Common.ItemAttribute.WeaponType) is string weaponName && (weaponName == "wand" || weaponName == "rod");

        public bool Use(ICombatActor actor, ICombatActor enemy, out CombatAttackType combatType)
        {
            combatType = new CombatAttackType(ShootType);

            if (actor is not IPlayer player) return false;
            if (!player.HasEnoughMana(ManaComsumption)) return false;

            var combat = new CombatAttackValue((ushort)(MaxHitChance / 2), MaxHitChance, Range, DamageType);

            if (DistanceCombatAttack.CalculateAttack(actor, enemy, combat, out var damage))
            {
                player.ConsumeMana(ManaComsumption);
                enemy.ReceiveAttack(actor, damage);
                return true;
            }

            return false;
        }
    }
}
