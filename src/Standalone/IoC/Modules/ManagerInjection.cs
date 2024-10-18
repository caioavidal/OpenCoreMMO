using Microsoft.Extensions.DependencyInjection;
using NeoServer.BuildingBlocks.Application;
using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Creature.Monster.Managers;
using NeoServer.Game.World.Models.Spawns;
using NeoServer.Modules.ItemManagement.DecayManagement;
using NeoServer.Modules.ItemManagement.DepotManagement;

namespace NeoServer.Server.Standalone.IoC.Modules;

public static class ManagerInjection
{
    public static IServiceCollection AddManagers(this IServiceCollection builder)
    {
        builder.AddSingleton<IGameServer, GameServer>();

        builder.AddSingleton<IItemDecayTracker, ItemDecayTracker>();
        builder.AddSingleton<IItemDecayProcessor, ItemDecayProcessor>();

        builder.AddSingleton<IMonsterDataManager, MonsterDataManager>();
        builder.AddSingleton<SpawnManager>();
        builder.AddSingleton<IDepotManager, DepotManager>();
        builder.AddSingleton<DepotTracker>();
        return builder;
    }
}