using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Creature.Model;
using NeoServer.Game.Creatures.Model.Monsters;
using NeoServer.Game.Creatures.Npcs;
using NeoServer.Game.DataStore;
using Serilog.Core;

namespace NeoServer.Game.Creatures
{
    public class NpcFactory : INpcFactory
    {
        private readonly IPathAccess pathAccess;
        private readonly Logger logger;

        public static INpcFactory Instance { get; private set; }

        public NpcFactory(CreaturePathAccess creaturePathAccess,
            Logger logger)
        {
            pathAccess = creaturePathAccess;
            this.logger = logger;
            Instance = this;

        }
        public INpc Create(string name, ISpawnPoint spawn = null)
        {
            var npcType = NpcStore.Data.Get(name);
            if (npcType is null)
            {
                logger.Warning($"Given npc name: {name} is not loaded");
                return null;
            }

            var npc = new Npc(npcType, pathAccess, new Outfit()
            {
                Addon = (byte)npcType.Look[Common.Creatures.LookType.Addon],
                LookType = (byte)npcType.Look[Common.Creatures.LookType.Type],
                Body = (byte)npcType.Look[Common.Creatures.LookType.Body],
                Feet = (byte)npcType.Look[Common.Creatures.LookType.Feet],
                Head = (byte)npcType.Look[Common.Creatures.LookType.Head],
                Legs = (byte)npcType.Look[Common.Creatures.LookType.Legs]
            }, npcType.MaxHealth);

            return npc;
        }

    }
}