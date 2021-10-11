using System;
using System.Linq;
using System.Text;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Items.Containers
{
    public class LootContainer : Container, ILootContainer
    {
        private readonly DateTime CreatedAt;

        public LootContainer(IItemType type, Location location, ILoot loot) : base(type, location)
        {
            Loot = loot;
            CreatedAt = DateTime.Now;
        }

        public ILoot Loot { get; }
        public bool LootCreated { get; private set; }

        public bool CanBeOpenedBy(IPlayer player)
        {
            return Allowed(player);
        }

        public void MarkAsLootCreated()
        {
            LootCreated = true;
        }

        private bool Allowed(IPlayer player)
        {
            if (Loot?.Owners is null || !Loot.Owners.Any()) return true;

            if (Loot.Owners.Contains(player)) return true;

            if ((DateTime.Now - CreatedAt).TotalSeconds > 10) return true; //todo: add 10 seconds to game configuration

            return false;
        }

        public bool CanBeMovedBy(IPlayer player)
        {
            return Allowed(player);
        }

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
                var itemType = item.ItemType?.Invoke();
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