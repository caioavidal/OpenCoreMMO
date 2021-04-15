using NeoServer.Game.Common;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.DataStore;
using NeoServer.Game.Contracts.Creatures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Game.Creatures.Npcs
{
    public class ShopperNpc : Npc, IShopperNpc
    {

        public event ShowShop OnShowShop;
        public ShopperNpc(INpcType type,ISpawnPoint spawnPoint, IOutfit outfit = null, uint healthPoints = 0) : base(type, spawnPoint, outfit, healthPoints)
        {
        }

        public IDictionary<ushort, IShopItem> ShopItems
        {
            get
            {
                if (!Metadata.CustomAttributes.TryGetValue("shop", out var shop)) return null;

                if (shop is not IDictionary<ushort, IShopItem> shopItems) return null;

                return shopItems;
            }
        }

        public override void SendMessageTo(ISociableCreature to, SpeechType type, IDialog dialog)
        {
            base.SendMessageTo(to, type, dialog);
            if (dialog.Action == "shop") ShowShopItems(to);

        }

        public virtual void ShowShopItems(ISociableCreature to)
        {
            if (to is not IPlayer player) return;

            if (ShopItems?.Values is not IEnumerable<IShopItem> shopItems) return;

            player.StartShopping(this);

            OnShowShop?.Invoke(this, to, shopItems);
        }

        public virtual void StopSellingToCustomer(ISociableCreature creature)
        {
            //todo: invoke event here
        }

        public bool BuyFromCustomer(ISociableCreature creature, IItemType item, byte amount)
        {
            var shopItems = ShopItems;
            if (shopItems is null) return false;

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

        public ulong CalculateCost(IItemType itemType, byte amount)
        {
            var shopItems = ShopItems;
            if (shopItems is null) return 0;

            if (!shopItems.TryGetValue(itemType.TypeId, out var shopItem)) return 0;

            return (shopItem?.BuyPrice ?? 0) * amount;
        }
    }
}
