using NeoServer.Game.Contracts.Items;
using System.Collections.Generic;

namespace NeoServer.Game.Contracts.Creatures
{
    public delegate void ShowShop(INpc npc, ISociableCreature to, IEnumerable<IShopItem> shopItems);
    public interface IShopperNpc : INpc
    {
        IDictionary<ushort, IShopItem> ShopItems { get; }

        event ShowShop OnShowShop;

        void StopSellingToCustomer(ISociableCreature creature);
        bool BuyFromCustomer(ISociableCreature creature, IItemType item, byte amount);
        bool Pay(ISociableCreature to, uint value);
        ulong CalculateCost(IItemType itemType, byte amount);
    }
}
