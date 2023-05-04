using System.Collections.Generic;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Items.Containers;
using NeoServer.Game.Items.Items.Containers.Container;

namespace NeoServer.Game.Items.Factories;

public class ContainerFactory : IFactory
{
    public event CreateItem OnItemCreated;


    public IItem Create(IItemType itemType, Location location, IEnumerable<IItem> children)
    {
        if (Depot.IsApplicable(itemType)) return new Depot(itemType, location, children);
        if (Container.IsApplicable(itemType)) return new Container(itemType, location, children);

        return null;
    }
}