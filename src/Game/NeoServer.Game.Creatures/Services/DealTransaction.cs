using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Creatures.Npcs.Shop;
using NeoServer.Game.DataStore;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Creatures.Events
{
    public class DealTransaction : IDealTransaction
    {
        private readonly IItemFactory itemFactory;

        public DealTransaction(IItemFactory itemFactory)
        {
            this.itemFactory = itemFactory;
        }

        public bool Buy(IPlayer buyer, IShopperNpc seller, IItemType itemType, byte amount)
        {
            if (buyer is null || seller is null || itemType is null || amount == 0) return false;

            var cost = seller.CalculateCost(itemType, amount);
            if (buyer.TotalMoney < cost) return false;

            RemoveCoins(buyer, cost);
            AddItems(buyer, seller, itemType.TypeId, amount);

            return true;
        }

        private void Sell(ISociableCreature part, ISociableCreature counterpart)
        {

        }

        private void AddItems(IPlayer player, INpc seller, ushort typeId, byte amount)
        {
            var item = itemFactory.Create(typeId, Location.Inventory(Common.Players.Slot.Backpack), null);

            if (item is ICumulative cumulative)
            {
                cumulative.Amount = amount;
                player.ReceivePurchasedItems(seller, item);
            }
            else
            {
                var items = new IItem[amount];
                items[0] = item;

                for (int i = 1; i < amount; i++)
                {
                    items[i] = itemFactory.Create(typeId, Location.Inventory(Common.Players.Slot.Backpack), null);
                }

                player.ReceivePurchasedItems(seller, items);
            }
        }
        private ulong RemoveCoins(IPlayer player, ulong amount)
        {
            if (player is null || amount == 0) return 0;

            var backpackSlot = player.Inventory?.BackpackSlot;

            ulong removedAmount = 0;

            if (backpackSlot is null) return removedAmount;

            var moneyMap = new SortedList<uint, List<ICoin>>(); //slot and item

            var containers = new Queue<IContainer>();
            containers.Enqueue(backpackSlot);

            while (containers.TryDequeue(out var container) && amount > 0)
            {
                foreach (var item in container.Items)
                {
                    if (item is IContainer childContainer)
                    {
                        containers.Enqueue(childContainer);
                        continue;
                    }
                    if (item is not ICoin coin) continue;

                    if (moneyMap.TryGetValue(coin.Worth, out var coinSlots))
                    {
                        coinSlots.Add(coin);
                        continue;
                    }

                    coinSlots = new List<ICoin>() { coin };
                    moneyMap.Add(coin.Worth, coinSlots);
                }

                foreach (var money in moneyMap)
                {
                    if (amount == 0) break;

                    foreach(var coin in money.Value)
                    {
                        if (amount == 0) break;

                        if (coin.Worth < amount)
                        {
                            container.RemoveItem(coin, coin.Amount);
                            amount -= coin.Worth;
                            removedAmount += coin.Worth;
                        }
                        else if (coin.Worth > amount)
                        {
                            uint worth = coin.Worth / coin.Amount;
                            uint removeCount = (uint)Math.Ceiling((decimal)(coin.Worth / worth));

                            var coinsToAdd = CoinCalculator.Calculate(CoinTypeStore.Data.Map, (worth * removeCount) - amount);

                            container.RemoveItem(coin, coin.Amount);

                            foreach (var coinToAdd in coinsToAdd)
                            {
                                var createdCoin = itemFactory.Create(coinToAdd.Item1, Location.Inventory(Common.Players.Slot.Backpack), null);
                                if (createdCoin is not ICoin newCoin) continue;
                                newCoin.Amount = coinToAdd.Item2;
                                backpackSlot.AddItem(newCoin, true);
                            }

                            return removedAmount + amount;
                        }
                        else
                        {
                            container.RemoveItem(coin, coin.Amount);
                            removedAmount += coin.Worth;
                        }
                    }
                }
            }

            return removedAmount;
        }
    }
}
