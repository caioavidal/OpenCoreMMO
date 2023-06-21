using System.Collections.Generic;
using System.Linq;
using NeoServer.Data.Extensions;
using NeoServer.Data.Model;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
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
            Charges = item is IChargeable chargeable ? chargeable.Charges : null
        };

        return itemModel;
    }

    public static IItem BuildContainer(List<PlayerDepotItemModel> items, int index, Location location,
        IContainer container, IItemFactory itemFactory, List<PlayerDepotItemModel> all)
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