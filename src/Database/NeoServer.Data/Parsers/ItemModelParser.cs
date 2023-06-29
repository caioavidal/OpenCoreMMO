﻿using System.Collections.Generic;
using System.Linq;
using NeoServer.Data.Entities;
using NeoServer.Data.Extensions;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Data.Parsers;

public static class ItemModelParser
{
    public static T ToPlayerItemModel<T>(IItem item) where T : PlayerItemBaseEntity, new()
    {
        var itemModel = new T
        {
            ServerId = (short)item.Metadata.TypeId,
            Amount = item is ICumulative cumulative ? cumulative.Amount : (short)1,
            DecayTo = item.Decay?.DecaysTo,
            DecayDuration = item.Decay?.Duration,
            DecayElapsed = item.Decay?.Elapsed,
            Charges = item is IChargeable chargeable ? chargeable.Charges : null
        };

        return itemModel;
    }

    public static IItem BuildContainer<T>(List<T> items, int index, Location location,
        IContainer container, IItemFactory itemFactory, List<T> all) where T : PlayerItemBaseEntity
    {
        if (items == null || index < 0) return container;

        var itemModel = items[index];

        var item = itemFactory
            .Create((ushort)itemModel.ServerId, location, itemModel.GetAttributes());

        if (item is IContainer childrenContainer)
        {
            var playerDepotItemModels = all.Where(c => c.ParentId.Equals(itemModel.Id)).ToList();
            childrenContainer.SetParent(container);
            container.AddItem(BuildContainer(playerDepotItemModels, playerDepotItemModels.Count - 1, location,
                childrenContainer, itemFactory, all));
        }
        else
        {
            container.AddItem(item);
        }

        return BuildContainer(items, --index, location, container, itemFactory, all);
    }
}