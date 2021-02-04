using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Contracts.Creatures
{
    public delegate void ShowShop(INpc npc, ISociableCreature to, IShopItem[] shopItems);
    public interface IShopperNpc : INpc
    {
        event ShowShop OnShowShop;
    }
}
