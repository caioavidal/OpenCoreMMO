using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.DataStore;
using System;
using System.Linq;
using System.Text;

namespace NeoServer.Game.Items.Items.Containers
{
    public class LootContainer : Container, ILootContainer
    {
        public LootContainer(IItemType type, Location location, ILoot loot) : base(type, location)
        {
            Loot = loot;
            CreatedAt = DateTime.Now;
        }

        private DateTime CreatedAt;
        public ILoot Loot { get; }
        public bool LootCreated { get; private set; }

        private bool Allowed(IPlayer player)
        {
            if (Loot?.Owners is null || !Loot.Owners.Any()) return true;

            if (Loot.Owners.Contains(player)) return true;

            if ((DateTime.Now - CreatedAt).TotalSeconds > 10) return true; //todo: add 10 seconds to game configuration

            return false;
        }
        public bool CanBeOpenedBy(IPlayer player) => Allowed(player);
        public bool CanBeMovedBy(IPlayer player) => Allowed(player);
        public void MarkAsLootCreated() => LootCreated = true;

        public override string ToString()
        {
            if (LootCreated) return base.ToString();
            
            var content = GetStringContent(Loot?.Items);
            if (string.IsNullOrWhiteSpace(content)) return "nothing";

            return content.Remove(content.Length - 2, 2);
        }

        private string GetStringContent(ILootItem[] items)
        {
            if (Loot is null) return string.Empty;
            var stringBuilder = new StringBuilder();

            foreach (var item in items)
            {
                var itemType = ItemTypeStore.Data.Get(item.ItemId);
                if (item.Amount > 1) stringBuilder.Append($"{item.Amount} {itemType.Name}");
                else stringBuilder.Append($"{itemType.Name}");

                stringBuilder.Append(", ");

                if (item.Items?.Any() ?? false)
                {
                    stringBuilder.Append(GetStringContent(item.Items));
                    stringBuilder.Append(", ");
                }
            }

            return stringBuilder.ToString();
        }
    }
}
