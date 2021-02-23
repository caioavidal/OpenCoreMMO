using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Server.Contracts;
using System.Collections.Generic;

namespace NeoServer.Server.Events.Creature
{
    public class CreatureDroppedLootEventHandler
    {
        private readonly IGameServer game;
        private readonly IItemFactory itemFactory;
        public CreatureDroppedLootEventHandler(IGameServer game, IItemFactory itemFactory)
        {
            this.game = game;
            this.itemFactory = itemFactory;
        }
        public void Execute(ICombatActor creature, ILoot loot, IEnumerable<ICreature> owners)
        {
         
        }    
    }
}