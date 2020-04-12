using Autofac;
using NeoServer.Game.Contracts;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Events
{
    public class EventSubscriber
    {
        private readonly IMap map;
        private IComponentContext container;

        public EventSubscriber(IMap map, IComponentContext container)
        {
            this.map = map;
            this.container = container;
        }

        public void AttachEvents()
        {
            map.CreatureAddedOnMap += (creature) => container.Resolve<PlayerAddedOnMapEventHandler>().Execute((IPlayer)creature);
            map.ThingRemovedFromTile += container.Resolve<ThingRemovedFromTileEventHandler>().Execute;
            map.ThingMovedOnFloor += container.Resolve<ThingMovedOnFloorEventHandler>().Execute;
        }
    }
}
