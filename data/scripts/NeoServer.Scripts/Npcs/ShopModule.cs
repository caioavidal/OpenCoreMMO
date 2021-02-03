using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Creatures.Npcs;
using NeoServer.Loaders.Npcs;
using NeoServer.Networking.Packets.Outgoing.Npc;
using NeoServer.Server.Items;
using NeoServer.Server.Model.Players.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Scripts.Npcs
{
    public class ShopModule
    {
        public static void LoadShopData(INpcType type, string jsonContent)
        {
            if (type is null || string.IsNullOrWhiteSpace(jsonContent)) return;

            var npc = JsonConvert.DeserializeObject<NpcData>(jsonContent);

            if (npc is null || npc.Shop is null) return;

            var items = new List<ShopItem>(npc.Shop.Length);
            foreach (var item in npc.Shop)
            {
                if (!ItemTypeData.InMemory.TryGetValue(item.Item, out var itemType)) continue;
                items.Add(new ShopItem(itemType, item.Buy, item.Sell));
            }

            type.CustomAttributes.Add("shop", items.ToArray());
        }

        public static void OpenShop(INpc npc, ICreature creature)
        {
            if (creature is not IPlayer player) return;
            if (npc is null) return;

            if (!Server.Game.Instance.CreatureManager.GetPlayerConnection(creature.CreatureId, out var connection)) return;

            if (!npc.Metadata.CustomAttributes.TryGetValue("shop", out var shop)) return;

            if (shop is not ShopItem[] shopItems) return;

            connection.OutgoingPackets.Enqueue(new OpenShopPacket(shopItems));
            connection.OutgoingPackets.Enqueue(new SaleItemListPacket(player, shopItems));
            connection.Send();
        }
    }

    internal class NpcData
    {
        public ShopData[] Shop { get; set; }
        internal class ShopData
        {
            public ushort Item { get; set; }
            public uint Sell { get; set; }
            public uint Buy { get; set; }
        }
    }
}
