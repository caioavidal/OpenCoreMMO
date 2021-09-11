using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Items.Containers;

namespace NeoServer.Game.Items.Factories
{
    public class ContainerFactory : IFactory
    {
        public event CreateItem OnItemCreated;


        public IItem Create(IItemType itemType, Location location)
        {
            if (PickupableContainer.IsApplicable(itemType)) return new PickupableContainer(itemType, location);
            if (Container.IsApplicable(itemType)) return new Container(itemType, location);
            if (Depot.IsApplicable(itemType)) return new Depot(itemType, location);

            return null;
        }
    }
}