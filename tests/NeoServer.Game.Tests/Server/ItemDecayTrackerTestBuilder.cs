using NeoServer.Application.Features.Decay;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Item.Services;
using NeoServer.Game.Item.Services.ItemTransform;
using NeoServer.Game.World.Services;

namespace NeoServer.Game.Tests.Server;

public class ItemDecayTrackerTestBuilder
{
    public static ItemDecayTracker Build(IMap map, IItemTypeStore itemTypeStore)
    {
        var mapService = new MapService(map);
        var itemFactory = ItemFactoryTestBuilder.Build();
        var itemTransformService = new ItemTransformService(itemFactory, map, mapService, itemTypeStore);
        return new ItemDecayTracker(itemTransformService);
    }
}