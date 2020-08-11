using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Body;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Enums.Players;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace NeoServer.Game.Items.Items
{
    public class ThrowableDistanceWeaponItem : CumulativeItem, IDistanceWeaponItem
    {
        public ThrowableDistanceWeaponItem(IItemType type, Location location, IDictionary<ItemAttribute, IConvertible> attributes) : base(type, location, attributes) { }
        public ThrowableDistanceWeaponItem(IItemType type, Location location, byte amount) : base(type, location, amount) { }
        public byte MaxAttackDistance { get; }

        public ImmutableHashSet<VocationType> AllowedVocations { get; }

        public static new bool IsApplicable(IItemType type) => type.Attributes.GetAttribute(Enums.ItemAttribute.WeaponType) == "distance" && type.HasFlag(Enums.ItemFlag.Stackable);
    }
}
