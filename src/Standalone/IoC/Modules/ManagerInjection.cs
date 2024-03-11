using Microsoft.Extensions.DependencyInjection;
using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Features.Creature;
using NeoServer.Application.Features.Item.Decay;
using NeoServer.Application.Features.Item.Depot;
using NeoServer.Application.Server;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Creature.Monster.Managers;
using NeoServer.Game.World.Models.Spawns;

namespace NeoServer.Server.Standalone.IoC.Modules;

public static class ManagerInjection
{
    public static IServiceCollection AddManagers(this IServiceCollection builder)
    {
        builder.AddSingleton<IGameServer, GameServer>();
        builder.AddSingleton<IGameCreatureManager, GameCreatureManager>();
        
        builder.AddSingleton<IItemDecayTracker, ItemDecayTracker>();
        builder.AddSingleton<IItemDecayProcessor, ItemDecayProcessor>();

        builder.AddSingleton<IMonsterDataManager, MonsterDataManager>();
        builder.AddSingleton<SpawnManager>();
        builder.AddSingleton<DepotManager>();
        builder.AddSingleton<DepotTracker>();
        return builder;
    }
}