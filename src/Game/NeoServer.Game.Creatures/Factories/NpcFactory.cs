using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Creatures.Model;
using NeoServer.Game.Creatures.Npcs;
using NeoServer.Game.Creatures.Npcs.Shop;
using NeoServer.Game.DataStore;
using Serilog;
using Serilog.Core;

namespace NeoServer.Game.Creatures.Factories
{
    public class NpcFactory : INpcFactory
    {
        private readonly IItemFactory _itemFactory;
        private readonly ILogger _logger;

        public NpcFactory(
            ILogger logger, IItemFactory itemFactory, ICreatureGameInstance creatureGameInstance)
        {
            this._logger = logger;
            Instance = this;
            this._itemFactory = itemFactory;
        }

        public static INpcFactory Instance { get; private set; }

        public INpc Create(string name, ISpawnPoint spawn = null)
        {
            var npcType = NpcStore.Data.Get(name);
            if (npcType is null)
            {
                _logger.Warning($"Given npc name: {name} is not loaded");
                return null;
            }

            var outfit = new Outfit
            {
                Addon = (byte) npcType.Look[LookType.Addon],
                LookType = (byte) npcType.Look[LookType.Type],
                Body = (byte) npcType.Look[LookType.Body],
                Feet = (byte) npcType.Look[LookType.Feet],
                Head = (byte) npcType.Look[LookType.Head],
                Legs = (byte) npcType.Look[LookType.Legs]
            };

            if (npcType.CustomAttributes.ContainsKey("shop"))
                return new ShopperNpc(npcType, spawn, outfit, npcType.MaxHealth)
                {
                    CreateNewItem = _itemFactory.Create
                };

            return new Npc(npcType, spawn, outfit, npcType.MaxHealth);
        }
    }
}