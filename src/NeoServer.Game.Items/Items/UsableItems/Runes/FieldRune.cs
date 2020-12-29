using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Runes;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Items.Items.UsableItems.Runes
{
    public class FieldRune : Rune, IFieldRune
    {
        public FieldRune(IItemType type, Location location, IDictionary<ItemAttribute, IConvertible> attributes) : base(type, location, attributes)
        {
        }

        public FieldRune(IItemType type, Location location, byte amount) : base(type, location, amount)
        {
        }

        public override ushort Duration => 2;
        public ushort Field => Metadata.Attributes.GetAttribute<ushort>(ItemAttribute.Field);

        public bool Use(ICreature usedBy, ITile tile)
        {
            if (tile is not IDynamicTile onTile) return false;
            var field = ItemFactory.Instance.Create(Field, onTile.Location, null);

            tile.AddItem(field);

            Reduce();
            return true;
        }
        public static new bool IsApplicable(IItemType type) => Rune.IsApplicable(type) && type.Attributes.HasAttribute(ItemAttribute.Field);

    }
}
