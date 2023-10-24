using Microsoft.Extensions.DependencyInjection;
using NeoServer.Application.Features.Decay;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Creature.Monster.Managers;
using NeoServer.Game.Systems.Depot;
using NeoServer.Game.World.Models.Spawns;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Managers;

namespace NeoServer.Server.Standalone.IoC.Modules;

public static class ManagerInjection
{
    public static IServiceCollection AddManagers(this IServiceCollection builder)
    {
        builder.AddSingleton<IGameServer, GameServer>();
        builder.AddSingleton<IGameCreatureManager, GameCreatureManager>();
        builder.AddSingleton<IItemDecayTracker, ItemDecayTracker>();


        builder.AddSingleton<IMonsterDataManager, MonsterDataManager>();
        builder.AddSingleton<SpawnManager>();
        builder.AddSingleton<DepotManager>();
        return builder;
    }
}