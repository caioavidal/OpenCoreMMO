using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Microsoft.Extensions.Caching.Memory;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Creatures;
using NeoServer.Game.World;
using NeoServer.Game.World.Map;
using NeoServer.Networking.Handlers;
using NeoServer.Server.Commands.Movements;
using NeoServer.Server.Commands.Player;
using NeoServer.Server.Common.Contracts.Tasks;
using NeoServer.Server.Standalone.IoC.Modules;
using NeoServer.Server.Tasks;
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

    public static IContainer BuildConfigurations()
    {
        var builder = new ContainerBuilder();

        var configuration = ConfigurationInjection.GetConfiguration();

        builder
            .AddConfigurations(configuration)
            .AddLogger(configuration);

        return builder.Build();
    }

    public static IContainer BuildAll()
    {
        var builder = new ContainerBuilder();

        //tools
        builder.RegisterType<PathFinder>().As<IPathFinder>().SingleInstance();
        builder.RegisterType<WalkToMechanism>().As<IWalkToMechanism>().SingleInstance();

        builder.RegisterPacketHandlers();

        builder.RegisterType<OptimizedScheduler>().As<IScheduler>().SingleInstance();
        builder.RegisterType<Dispatcher>().As<IDispatcher>().SingleInstance();
        builder.RegisterType<PersistenceDispatcher>().As<IPersistenceDispatcher>().SingleInstance();

        //world
        builder.RegisterType<Map>().As<IMap>().SingleInstance();
        builder.RegisterType<World>().SingleInstance();

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
            .AddDataStores();

        //creature
        builder.RegisterType<CreatureGameInstance>().As<ICreatureGameInstance>().SingleInstance();

        builder.RegisterInstance(new MemoryCache(new MemoryCacheOptions())).As<IMemoryCache>();

        return builder.Build();
    }

    private static void RegisterPacketHandlers(this ContainerBuilder builder)
    {
        var assemblies = Assembly.GetAssembly(typeof(PacketHandler));
        builder.RegisterAssemblyTypes(assemblies).SingleInstance();
    }

    private static ContainerBuilder AddCommands(this ContainerBuilder builder)
    {
        var assembly = Assembly.GetAssembly(typeof(PlayerLogInCommand));
        builder.RegisterAssemblyTypes(assembly);
        return builder;
    }
}