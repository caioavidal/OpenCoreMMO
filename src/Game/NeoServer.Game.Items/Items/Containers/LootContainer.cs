using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using System;
using System.Linq;

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
    }
}
