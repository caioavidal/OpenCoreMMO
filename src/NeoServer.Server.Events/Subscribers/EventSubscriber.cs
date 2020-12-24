using Autofac;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Creatures.Spells;

namespace NeoServer.Server.Events
{
    public class EventSubscriber
    {
        private readonly IMap map;
        private IComponentContext container;
        private readonly ILiquidPoolFactory itemFactory;

        public EventSubscriber(IMap map, IComponentContext container, ILiquidPoolFactory itemFactory)
        {
            this.map = map;
            this.container = container;
            this.itemFactory = itemFactory;   
        }

        public virtual void AttachEvents()
        {
            map.OnCreatureAddedOnMap += (creature, cylinder) => container.Resolve<PlayerAddedOnMapEventHandler>().Execute(creature, cylinder);
            map.OnThingRemovedFromTile += container.Resolve<ThingRemovedFromTileEventHandler>().Execute;
            map.OnCreatureMoved += container.Resolve<CreatureMovedOnFloorEventHandler>().Execute;
            map.OnThingMovedFailed += container.Resolve<InvalidOperationEventHandler>().Execute;
            map.OnThingAddedToTile += container.Resolve<ThingAddedToTileEventHandler>().Execute;
            map.OnThingUpdatedOnTile += container.Resolve<ThingUpdatedOnTileEventHandler>().Execute;
            BaseSpell.OnSpellInvoked += container.Resolve<SpellInvokedEventHandler>().Execute;
            itemFactory.OnItemCreated += container.Resolve<ItemCreatedEventHandler>().Execute;
        }
    }
}
