using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Item.Items.Attributes;

namespace NeoServer.Game.Item.Factories.AttributeFactory;

public class DecayableFactory
{
    public static IDecayable CreateIfItemIsDecayable(IItem item)
    {
        if (Guard.AnyNull(item)) return null;

        return item.HasDecayBehavior ? new Decayable(item) : null;
    }
}