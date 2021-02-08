using NeoServer.Game.Contracts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Contracts.Creatures
{
    public delegate void ShowShop(INpc npc, ISociableCreature to, IEnumerable<IShopItem> shopItems);
    public interface IShopperNpc : INpc
    {
        event ShowShop OnShowShop;

        void StopSellingToCustomer(ISociableCreature creature);
        bool BuyFromCustomer(ISociableCreature creature, IItemType item, byte amount);
        bool Pay(ISociableCreature to, uint value);
    }
}
