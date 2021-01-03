using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Players;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Body;
using System.Collections.Immutable;

namespace NeoServer.Game.Items.Items
{

    public class Wand : MoveableItem, IDistanceWeaponItem
    {
        public Wand(IItemType type, Location location) : base(type, location)
        {
        }
        public ImmutableHashSet<VocationType> AllowedVocations { get; }
        public byte Range => Metadata.Attributes.GetAttribute<byte>(Common.ItemAttribute.Range);

        public static bool IsApplicable(IItemType type) => type.Attributes.GetAttribute(Common.ItemAttribute.WeaponType) == "wand";

        public bool Use(ICombatActor actor, ICombatActor enemy, out CombatAttackType combatType)
        {
            var result = false;
            combatType = new CombatAttackType(ShootType.Fire);

            var maxDamage = 20;

            enemy.ReceiveAttack(enemy, new CombatDamage(20, DamageType.FireField));

            return true;
        }
    }
}
