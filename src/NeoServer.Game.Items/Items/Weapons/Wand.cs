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
using System;
using System.Collections.Immutable;

namespace NeoServer.Game.Items.Items
{

    public class Wand : MoveableItem, IDistanceWeaponItem
    {
        public Wand(IItemType type, Location location) : base(type, location)
        {
        }
        public ImmutableHashSet<VocationType> AllowedVocations { get; }
        public byte Range => Metadata.Attributes.GetAttribute<byte>(Enums.ItemAttribute.Range);

        public static bool IsApplicable(IItemType type) => type.Attributes.GetAttribute(Enums.ItemAttribute.WeaponType) == "wand";

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
