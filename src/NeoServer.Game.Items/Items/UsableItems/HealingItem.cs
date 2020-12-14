using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Server.Helpers;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Items.Items.UsableItems
{
    public class HealingItem: CumulativeUsableOnItem
    {
        public HealingItem(IItemType type, Location location, IDictionary<ItemAttribute, IConvertible> attributes) : base(type, location, attributes)
        {
        }

        public ushort Min => Metadata.Attributes.GetInnerAttributes(Common.ItemAttribute.Healing)?.GetAttribute<ushort>(ItemAttribute.Min) ?? 0;
        public ushort Max => Metadata.Attributes.GetInnerAttributes(Common.ItemAttribute.Healing)?.GetAttribute<ushort>(ItemAttribute.Max) ?? 0;

        public override void UseOn(IPlayer player, IMap map, IThing thing)
        {
            if (thing is not ICombatActor creature) return;
            if (Max == 0) return;

            var value = (ushort) ServerRandom.Random.Next(minValue: Min, maxValue: Max);
            creature.Heal(value);        
        }
        public static new bool IsApplicable(IItemType type) => (type.Attributes?.HasAttribute(ItemAttribute.Healing) ?? false) && CumulativeUsableOnItem.IsApplicable(type);

    }
}
