using NeoServer.Game.Contracts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Creatures.Npcs
{
    public record ShopItem(IItemType Item, uint BuyPrice, uint SellPrice)
    {
    }
}
