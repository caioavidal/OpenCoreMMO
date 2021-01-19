using NeoServer.Data.Model;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Data.Parsers
{
    public class ItemModelParser
    {
        public static PlayerDepotItemModel ToModel(IItem item)
        {
            var itemModel = new PlayerDepotItemModel()
            {
                ServerId = (short)item.Metadata.TypeId,
                Amount = item is ICumulative cumulative ? cumulative.Amount : 1,
            };

            return itemModel;
        }

        public static IItem BuildContainer(List<PlayerDepotItemModel> items, int index, Location location, IContainer container, IItemFactory itemFactory, List<PlayerDepotItemModel> all)
        {
            if (items == null || items.Count == index)
            {
                return container;
            }

            var itemModel = items[index];

            var item = itemFactory.Create((ushort)itemModel.ServerId, location, new Dictionary<ItemAttribute, IConvertible>()
                        {
                            {ItemAttribute.Count, itemModel.Amount }
                        });

            if (item is IContainer childrenContainer)
            {
                childrenContainer.SetParent(container);
                container.AddItem(BuildContainer(all.Where(c => c.ParentId.Equals(itemModel.Id)).ToList(), 0, location, childrenContainer, itemFactory, all));
            }
            else
            {
                container.AddItem(item);

            }
            return BuildContainer(items, ++index, location, container, itemFactory, all);
        }

    }
}
