using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Data.Model;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Data.Parsers;

public class ItemModelParser
{
    public static PlayerDepotItemModel ToModel(IItem item)
    {
        var itemModel = new PlayerDepotItemModel
        {
            ServerId = (short)item.Metadata.TypeId,
            Amount = item is ICumulative cumulative ? cumulative.Amount : (short)1,
            DecayTo = item.Decay?.DecaysTo,
            DecayDuration = item.Decay?.Duration,
            DecayElapsed = item.Decay?.Elapsed,
        };

        return itemModel;
    }

    public static IItem BuildContainer(List<PlayerDepotItemModel> items, int index, Location location,
        IContainer container, IItemFactory itemFactory, List<PlayerDepotItemModel> all)
    {
        if (items == null || index < 0) return container;

        var itemModel = items[index];


        var attributes = new Dictionary<ItemAttribute, IConvertible>
        {
            { ItemAttribute.Count, itemModel.Amount }
        };
        
        if (itemModel.DecayDuration > 0)
        {
            attributes.Add(ItemAttribute.DecayTo, itemModel.DecayTo);
            attributes.Add(ItemAttribute.DecayElapsed, itemModel.DecayElapsed);
            attributes.Add(ItemAttribute.Duration, itemModel.DecayDuration);
        }
        
        var item = itemFactory
            .Create((ushort)itemModel.ServerId, location, attributes);
        
        if (item is IContainer childrenContainer)
        {
            var playerDepotItemModels = all.Where(c => c.ParentId.Equals(itemModel.Id)).ToList();
            childrenContainer.SetParent(container);
            container.AddItem(BuildContainer(playerDepotItemModels, 0, location,
                childrenContainer, itemFactory, all));
        }
        else
        {
            container.AddItem(item);
        }

        return BuildContainer(items, --index, location, container, itemFactory, all);
    }
}