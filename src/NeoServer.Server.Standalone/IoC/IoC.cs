using Autofac;
using Microsoft.Extensions.Caching.Memory;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Creature;
using NeoServer.Game.World;
using NeoServer.Game.World.Map;
using NeoServer.Server.Commands;
using NeoServer.Server.Commands.Movement;
using NeoServer.Server.Contracts.Tasks;
using NeoServer.Server.Handlers;
using NeoServer.Server.Standalone.IoC.Modules;
using NeoServer.Server.Tasks;
using System.Reflection;

namespace NeoServer.Server.Standalone.IoC
{
    public static class Container
    {
        public static IContainer CompositionRoot()
        {
            var builder = new ContainerBuilder(); 
           
            //tools
            builder.RegisterType<Game.World.Map.PathFinder>().As<IPathFinder>().SingleInstance();
            builder.RegisterType<WalkToMechanism>().As<IWalkToMechanism>().SingleInstance();

            builder.RegisterPacketHandlers();

            builder.RegisterType<OptimizedScheduler>().As<IScheduler>().SingleInstance();
            builder.RegisterType<Dispatcher>().As<IDispatcher>().SingleInstance();

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
                   .AddJobs();

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
}
