using NeoServer.Game.Common;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Items.Items.UsableItems
{
    public class HealingItem : Cumulative, IConsumable
    {
        public HealingItem(IItemType type, Location location, IDictionary<ItemAttribute, IConvertible> attributes) : base(type, location, attributes)
        {
        }

        public ushort Min => Metadata.Attributes.GetInnerAttributes(ItemAttribute.Healing)?.GetAttribute<ushort>(ItemAttribute.Min) ?? 0;
        public ushort Max => Metadata.Attributes.GetInnerAttributes(ItemAttribute.Healing)?.GetAttribute<ushort>(ItemAttribute.Max) ?? 0;
        public string Type => Metadata.Attributes.GetAttribute(ItemAttribute.Healing);
      
        public event Use OnUsed;

        public static new bool IsApplicable(IItemType type) => (type.Attributes?.HasAttribute(ItemAttribute.Healing) ?? false) && Cumulative.IsApplicable(type) && UseableOnItem.IsApplicable(type);

        public void Use(IPlayer usedBy, ICreature creature)
        {
            if (creature is not ICombatActor actor) return;
            if (Max == 0) return;

            var value = (ushort)GameRandom.Random.Next(minValue: Min, maxValue: Max);

            if (Type.Equals("hp", StringComparison.InvariantCultureIgnoreCase))
            {
                actor.Heal(value);
            }
            else if (creature is IPlayer player)
            {
                player.HealMana(value);
            }

            Reduce();

            OnUsed?.Invoke(usedBy, creature, this);
        }
    }
}