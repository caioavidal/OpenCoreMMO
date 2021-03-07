using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.DataStore;
using NeoServer.Server.Model.Players.Contracts;
using System.Collections.Generic;

namespace NeoServer.Game.Creatures.Events
{
    public class DealTransaction : IDealTransaction
    {
        private readonly ICoinTransaction coinTransaction;
        private readonly IItemFactory itemFactory;

        public DealTransaction(IItemFactory itemFactory, ICoinTransaction coinTransaction)
        {
            this.itemFactory = itemFactory;
            this.coinTransaction = coinTransaction;
        }

        public bool Buy(IPlayer buyer, IShopperNpc seller, IItemType itemType, byte amount)
        {
            if (buyer is null || seller is null || itemType is null || amount == 0) return false;

            var cost = seller.CalculateCost(itemType, amount);
            if (buyer.TotalMoney < cost) return false;

            var amountToAddToInventory = buyer.Inventory.CanAddItem(itemType).Value;
            var amountToAddToBackpack = buyer.Inventory.BackpackSlot?.CanAddItem(itemType).Value ?? 0;

            if (amount > amountToAddToBackpack + amountToAddToInventory) return false;

            var removedAmount = coinTransaction.RemoveCoins(buyer, cost, out var change);

            if (removedAmount < cost) buyer.WithdrawFromBank(cost - removedAmount);

            var saleContract = new SaleContract(itemType.TypeId, amount, amountToAddToInventory, amountToAddToBackpack);

            AddItems(buyer, seller, saleContract);

            coinTransaction.AddCoins(buyer, change);
            
            return true;
        }

        private void Sell(ISociableCreature part, ISociableCreature counterpart)
        {

        }

        private void AddItems(IPlayer player, INpc seller, SaleContract saleContract)
        {
            
            var item = itemFactory.Create(saleContract.TypeId, Location.Inventory(Common.Players.Slot.Backpack), null);

            if (item is ICumulative cumulative)
            {
                cumulative.Amount = saleContract.Amount;
                player.ReceivePurchasedItems(seller, saleContract, item);
            }
            else
            {
                var items = new IItem[saleContract.Amount];
                items[0] = item;

                for (int i = 1; i < saleContract.Amount; i++)
                {
                    items[i] = itemFactory.Create(saleContract.TypeId, Location.Inventory(Common.Players.Slot.Backpack), null);
                }

                player.ReceivePurchasedItems(seller, saleContract, items);
            }
        }
      
        public IEnumerable<IItem> CreateCoins(ulong amount)
        {
            var coinsToAdd = CoinCalculator.Calculate(CoinTypeStore.Data.Map, amount);

            foreach (var coinToAdd in coinsToAdd)
            {
                var createdCoin = itemFactory.Create(coinToAdd.Item1, Location.Inventory(Common.Players.Slot.Backpack), null);
                if (createdCoin is not ICoin newCoin) continue;
                newCoin.Amount = coinToAdd.Item2;

                yield return newCoin;
            }
        }

    }

}
