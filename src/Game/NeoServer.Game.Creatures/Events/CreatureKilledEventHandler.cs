using NeoServer.Game.Common.Item;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Creatures.Enums;

namespace NeoServer.Game.Creatures.Events
{
    public class CreatureKilledEventHandler : IGameEventHandler
    {
        private readonly IItemFactory itemFactory;
        private readonly IMap map;
        private readonly ILiquidPoolFactory liquidPoolFactory;
        public CreatureKilledEventHandler(IItemFactory itemFactory, IMap map, ILiquidPoolFactory liquidPoolFactory)
        {
            this.itemFactory = itemFactory;
            this.map = map;
            this.liquidPoolFactory = liquidPoolFactory;
        }
        public void Execute(ICreature creature)
        {
            CreateCorpse(creature);
            CreateBlood(creature);
        }

        private void CreateCorpse(ICreature creature)
        {
            var corpse = itemFactory.Create(creature.CorpseType, creature.Location, null);
            creature.Corpse = corpse;

            if (creature is IWalkableCreature walkable)
            {
                walkable.Tile.AddItem(corpse);
                map.RemoveCreature(creature);
            }
        }
        private void CreateBlood(ICreature creature)
        {
            if (creature is not ICombatActor victim) return;
            var liquidColor = victim.Blood switch
            {
                BloodType.Blood => LiquidColor.Red,
                BloodType.Slime => LiquidColor.Green
            };

            var pool = liquidPoolFactory.Create(victim.Location, liquidColor);

            map.CreateBloodPool(pool, victim.Tile);
        }
    }
}
