using NeoServer.Game.Common;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts.Chats;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Creatures.Model.Bases;
using NeoServer.Game.DataStore;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Creatures.Npcs
{
    public class ShopperNpc : Npc, IShopperNpc
    {
        public event ShowShop OnShowShop;
        public ShopperNpc(INpcType type, IPathAccess pathAccess, IOutfit outfit = null, uint healthPoints = 0) : base(type, pathAccess, outfit, healthPoints)
        {
        }

        public override void SendMessageTo(ISociableCreature to, SpeechType type, INpcDialog dialog)
        {
            base.SendMessageTo(to, type, dialog);
            if (dialog.Action == "shop") ShowShopItems(to);

        }

        public virtual void ShowShopItems(ISociableCreature to)
        {
            if (to is not IPlayer player) return;

            if (!Metadata.CustomAttributes.TryGetValue("shop", out var shop)) return;

            if (shop is not IDictionary<ushort, IShopItem> shopItems) return;

            player.StartShopping(this);

            OnShowShop?.Invoke(this, to, shopItems.Values);
        }

        public virtual void StopSellingToCustomer(ISociableCreature creature)
        {
            //todo: invoke event here
        }

        public bool BuyFromCustomer(ISociableCreature creature, IItemType item, byte amount)
        {
            if (!Metadata.CustomAttributes.TryGetValue("shop", out var shop)) return false;
            if (shop is not IDictionary<ushort, IShopItem> shopItems) return false;

            if (!shopItems.TryGetValue(item.TypeId, out var shopItem)) return false;

            return Pay(creature, shopItem.SellPrice * amount);
        }

        public bool Pay(ISociableCreature to, uint value)
        {
            if (to is not IPlayer player) return false;
            if (value == 0) return false;

            var coins = CoinCalculator.Calculate(CoinTypeStore.Data.Map, value);

            IItem[] items = new IItem[coins.Count()];

            var i = 0;
            foreach (var coin in coins)
            {
                var (coinType, amount) = coin;

                var item = CreateNewItem(coinType, Common.Location.Structs.Location.Inventory(Common.Players.Slot.Backpack), new Dictionary<ItemAttribute, IConvertible>() { { ItemAttribute.Count, amount } });

                if (item is null) continue;
                items[i++] = item;
            }

            player.ReceivePayment(items, value);

            return true;
        }

        public void Sell(ISociableCreature to, IItemType itemType, byte amount)
        {
            if (to is not IPlayer player) return;

            var totalCost = CalculateCost(itemType, amount);
            var item = CreateNewItem(itemType.TypeId, Common.Location.Structs.Location.Inventory(Common.Players.Slot.Backpack), null);

            if (item is ICumulative cumulative)
            {
                cumulative.Amount = amount;
                player.ReceivePurchasedItems(this, totalCost, item);
            }
            else
            {
                var items = new IItem[amount];
                items[0] = item;

                for (int i = 1; i < amount; i++)
                {
                    items[i] = CreateNewItem(itemType.TypeId, Common.Location.Structs.Location.Inventory(Common.Players.Slot.Backpack), null);
                }

                player.ReceivePurchasedItems(this, totalCost, items);
            }
        }

        public ulong CalculateCost(IItemType itemType, byte amount)
        {
            if (!Metadata.CustomAttributes.TryGetValue("shop", out var shop)) return 0;
            if (shop is not IDictionary<ushort, IShopItem> shopItems) return 0;

            if (!shopItems.TryGetValue(itemType.TypeId, out var shopItem)) return 0;


            return (shopItem?.BuyPrice ?? 0) * amount;
        }
    }
}
