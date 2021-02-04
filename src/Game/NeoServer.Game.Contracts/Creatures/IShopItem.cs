using NeoServer.Game.Contracts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface IShopItem
    {
        IItemType Item { get; } 
        uint BuyPrice { get; }
        uint SellPrice { get; }
    }
}
