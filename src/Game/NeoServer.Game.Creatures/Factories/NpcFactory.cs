using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Creature.Model;
using NeoServer.Game.Creatures.Npcs;
using NeoServer.Game.DataStore;
using Serilog.Core;

namespace NeoServer.Game.Creatures
{
    public class NpcFactory : INpcFactory
    {
        private readonly IItemFactory itemFactory;
        private readonly Logger logger;
        private readonly ICreatureGameInstance creatureGameInstance;

        public static INpcFactory Instance { get; private set; }

        public NpcFactory(
            Logger logger, IItemFactory itemFactory, ICreatureGameInstance creatureGameInstance)
        {
            this.logger = logger;
            Instance = this;
            this.itemFactory = itemFactory;
            this.creatureGameInstance = creatureGameInstance;
        }
        public INpc Create(string name, ISpawnPoint spawn = null)
        {
            var npcType = NpcStore.Data.Get(name);
            if (npcType is null)
            {
                logger.Warning($"Given npc name: {name} is not loaded");
                return null;
            }

            var outfit = new Outfit()
            {
                Addon = (byte)npcType.Look[Common.Creatures.LookType.Addon],
                LookType = (byte)npcType.Look[Common.Creatures.LookType.Type],
                Body = (byte)npcType.Look[Common.Creatures.LookType.Body],
                Feet = (byte)npcType.Look[Common.Creatures.LookType.Feet],
                Head = (byte)npcType.Look[Common.Creatures.LookType.Head],
                Legs = (byte)npcType.Look[Common.Creatures.LookType.Legs]
            };

            if (npcType.CustomAttributes.ContainsKey("shop"))
            {
                return new ShopperNpc(npcType, spawn, outfit, npcType.MaxHealth)
                {
                    CreateNewItem = itemFactory.Create
                };
            }

            return new Npc(npcType, spawn, outfit, npcType.MaxHealth);
        }

    }
}