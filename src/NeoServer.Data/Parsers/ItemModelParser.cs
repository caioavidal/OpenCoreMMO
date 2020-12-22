using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Server.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Data.Parsers
{
    public class ItemModelParser
    {
        public static IItemModel ToModel(IItem item)
        {
            var itemModel = new ItemModel()
            {
                ServerId = item.Metadata.TypeId,
                Amount = item is ICumulative cumulative ? cumulative.Amount : 1
            };

            if (item is IContainer container)
            {
                var items = new List<IItemModel>();
                foreach (var i in container.Items)
                {
                    items.Add(ToModel(i));
                }
                itemModel.Items = items;
            }
            return itemModel;
        }
        public static IItem BuildContainer(List<IItemModel> items,int index, Location location, IContainer container, IItemFactory itemFactory)
        {
            if (items == null || items.Count == index)
            {
                return container;
            }

            var itemModel = items[index];

            var item = itemFactory.Create(itemModel.ServerId, location, new Dictionary<ItemAttribute, IConvertible>()
                        {
                            {ItemAttribute.Count, itemModel.Amount }
                        });

            if (item is IContainer childrenContainer)
            {
                childrenContainer.SetParent(container);
                container.AddThing(BuildContainer(itemModel.Items?.Reverse().ToList(), 0, location, childrenContainer, itemFactory));
            }
            else
            {
                container.AddThing(item);

            }
            return BuildContainer(items, ++index, location, container, itemFactory);
        }

    }
}
