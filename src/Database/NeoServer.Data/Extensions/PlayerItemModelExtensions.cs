using System;
using System.Collections.Generic;
using NeoServer.Data.Model;
using NeoServer.Game.Common.Item;

namespace NeoServer.Data.Extensions;

public static class PlayerItemModelExtensions
{
    public static Dictionary<ItemAttribute, IConvertible> GetAttributes(this PlayerItemModel itemModel)
    {
        var attributes = new Dictionary<ItemAttribute, IConvertible>
        {
            { ItemAttribute.Count, itemModel.Amount }
        };

        if (itemModel.Charges > 0) attributes.Add(ItemAttribute.Charges, itemModel.Charges);

        if (itemModel.DecayDuration > 0)
        {
            attributes.Add(ItemAttribute.DecayTo, itemModel.DecayTo);
            attributes.Add(ItemAttribute.DecayElapsed, itemModel.DecayElapsed);
            attributes.Add(ItemAttribute.Duration, itemModel.DecayDuration);
        }

        return attributes;
    }
}