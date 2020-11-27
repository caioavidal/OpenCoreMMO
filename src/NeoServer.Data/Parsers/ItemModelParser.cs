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
                ServerId = item.ClientId,
                Amount = item is ICumulativeItem cumulative ? cumulative.Amount : 1
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
    }
}
