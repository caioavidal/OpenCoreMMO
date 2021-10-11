using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Items.Items.Attributes;

namespace NeoServer.Game.Items.Factories.AttributeFactory
{
    public  class DecayableFactory
    {
        public static IDecayable Create(IItem item)
        {
            if (Guard.AnyNull(item)) return null;

            var itemType = item.Metadata;

            var hasShowDuration =
                itemType.Attributes.TryGetAttribute<ushort>(ItemAttribute.ShowDuration, out var showDuration);
            var hasDuration = itemType.Attributes.TryGetAttribute<ushort>(ItemAttribute.Duration, out var duration);

            if (!hasShowDuration && !hasDuration) return null;

            return new Decayable(item);
        }
        public static bool HasDecayable(IItemType itemType)
        {
            if (Guard.AnyNull(itemType)) return false;

            var hasShowDuration =
                itemType.Attributes.TryGetAttribute<ushort>(ItemAttribute.ShowDuration, out var showDuration);
            var hasDuration = itemType.Attributes.TryGetAttribute<ushort>(ItemAttribute.Duration, out var duration);

            if (!hasShowDuration && !hasDuration) return false;

            return true;
        }
    }
}