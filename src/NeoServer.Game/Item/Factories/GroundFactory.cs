using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Item.Items;

namespace NeoServer.Game.Item.Factories;

public class GroundFactory : IFactory
{
    public event CreateItem OnItemCreated;


    public IItem Create(IItemType itemType, Location location)
    {
        if (Ground.IsApplicable(itemType)) return new Ground(itemType, location);

        return null;
    }
}