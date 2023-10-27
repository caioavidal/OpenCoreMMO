using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using NeoServer.Application.Common.PacketHandler;
using NeoServer.Application.Infrastructure.Thread;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Creature;
using NeoServer.Game.World;
using NeoServer.Game.World.Map;
using NeoServer.Networking.Handlers;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Commands.Movements;
using NeoServer.Server.Commands.Player;
using NeoServer.Server.Common.Contracts.Tasks;
using NeoServer.Server.Standalone.IoC.Modules;
using NeoServer.Shared.IoC.Modules;
using PathFinder = NeoServer.Game.World.Map.PathFinder;

namespace NeoServer.Server.Standalone.IoC;

public static class Container
{
    internal static Assembly[] AssemblyCache => AppDomain.CurrentDomain.GetAssemblies().AsParallel().Where(assembly =>
        !assembly.IsDynamic &&
        !assembly.FullName.StartsWith("System.") &&
        !assembly.FullName.StartsWith("Microsoft.") &&
        !assembly.FullName.StartsWith("Windows.") &&
        !assembly.FullName.StartsWith("mscorlib,") &&
        !assembly.FullName.StartsWith("Serilog,") &&
        !assembly.FullName.StartsWith("Autofac,") &&
        !assembly.FullName.StartsWith("netstandard,")).ToArray();

    public static IServiceProvider BuildConfigurations()
    {
        var builder = new ServiceCollection();

        var configuration = ConfigurationInjection.GetConfiguration();

        builder
            .AddConfigurations(configuration)
            .AddLogger(configuration);

        return builder
            .BuildServiceProvider()
            .Verify(builder);
    }

    public static IServiceProvider BuildAll()
    {
        var builder = new ServiceCollection();

        builder.AddMediator();

        //tools
        builder.AddSingleton<IPathFinder, PathFinder>()
            .AddSingleton<IWalkToMechanism, WalkToMechanism>()

            //thread management
            .AddSingleton<IScheduler, OptimizedScheduler>()
            .AddSingleton<IDispatcher, Dispatcher>()
            .AddSingleton<IPersistenceDispatcher, PersistenceDispatcher>();

        //world
        builder.AddSingleton<IMap, Map>();
        builder.AddSingleton<World>();

        var configuration = ConfigurationInjection.GetConfiguration();

        builder.AddFactories()
            .AddServices()
            .AddLoaders()
            .AddDatabases(configuration)
            .AddRepositories()
            .AddConfigurations(configuration)
            .AddNetwork()
            .AddEvents()
            .AddManagers()
            .AddLogger(configuration)
            .AddCommands()
            .AddLua()
            .AddJobs()
            .AddCommands()
            .AddDataStores()
            .RegisterPacketHandlers();

        //creature
        builder.AddSingleton<ICreatureGameInstance, CreatureGameInstance>();

        builder.AddSingleton<IMemoryCache>(new MemoryCache(new MemoryCacheOptions()));

        return builder
            .BuildServiceProvider()
            .Verify(builder);
    }

    private static IServiceCollection RegisterPacketHandlers(this IServiceCollection builder)
    {
        _ = Assembly.GetAssembly(typeof(PacketHandler));
        return builder.RegisterAssembliesByInterface(typeof(IPacketHandler));
    }

    private static IServiceCollection AddCommands(this IServiceCollection builder)
    {
        var assembly = Assembly.GetAssembly(typeof(PlayerLogInCommand));
        return builder.RegisterAssemblyTypes(assembly);
    }
}