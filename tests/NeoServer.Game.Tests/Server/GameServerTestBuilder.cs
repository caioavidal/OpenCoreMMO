using Moq;
using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Features.Creature;
using NeoServer.Application.Infrastructure.Thread;
using NeoServer.Application.Server;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using Serilog;

namespace NeoServer.Game.Tests.Server;

public static class GameServerTestBuilder
{
    public static IGameServer Build(IMap map)
    {
        var logger = new Mock<ILogger>().Object;
        var dispatcher = new Dispatcher(logger);

        var decayableItemManager = ItemDecayServiceTestBuilder.BuildTracker();
        var persistenceDispatcher = new PersistenceDispatcher(logger);

        var gameServer = new GameServer(map, dispatcher, new OptimizedScheduler(dispatcher),
            new GameCreatureManager(new Mock<ICreatureGameInstance>().Object, map, logger), decayableItemManager,
            persistenceDispatcher);

        return gameServer;
    }
}