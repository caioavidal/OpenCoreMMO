using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Item.Items.Attributes;

namespace NeoServer.Modules.ItemManagement.Factories.AttributeFactory;

public class ProtectionFactory
{
    public static IProtection Create(IItem item)
    {
        if (item.Metadata.Attributes.DamageProtection is not { } damageProtection) return null;
        if (!damageProtection.Any()) return null;

        return new Protection(item);
    }
}