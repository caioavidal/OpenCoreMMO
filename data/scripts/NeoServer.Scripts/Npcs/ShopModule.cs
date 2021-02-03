using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Loaders.Npcs;
using NeoServer.Networking.Packets.Outgoing.Npc;
using NeoServer.Server.Items;
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

            if (npc is null) return;

            type.CustomAttributes.Add("shop", npc.Shop.Select(x => (x.Item, x.Buy, x.Sell)).ToArray());
        }

        public static void OpenShop(INpc npc, ICreature creature)
        {
            if (!Server.Game.Instance.CreatureManager.GetPlayerConnection(creature.CreatureId, out var connection)) return;

            if (!npc.Metadata.CustomAttributes.TryGetValue("shop", out var shop)) return;

            if (shop is not (ushort, uint, uint)[] itemsShop) return;

            var items = new List<(IItemType, uint, uint)>(itemsShop.Length);
            foreach (var item in itemsShop)
            {
                if (!ItemTypeData.InMemory.TryGetValue(item.Item1, out var itemType)) continue;
                items.Add((itemType, item.Item2, item.Item3));
            }

            connection.OutgoingPackets.Enqueue(new OpenShopPacket(items.ToArray()));
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
