using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using NeoServer.Data.Contexts;
using NeoServer.Game.Common;
using NeoServer.Game.World.Spawns;
using NeoServer.Loaders.Interfaces;
using NeoServer.Loaders.Items;
using NeoServer.Loaders.Monsters;
using NeoServer.Loaders.Spawns;
using NeoServer.Loaders.Spells;
using NeoServer.Loaders.Vocations;
using NeoServer.Loaders.World;
using NeoServer.Networking.Listeners;
using NeoServer.Scripts.Lua;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Tasks;
using NeoServer.Server.Compiler;
using NeoServer.Server.Configurations;
using NeoServer.Server.Events.Subscribers;
using NeoServer.Server.Helpers.Extensions;
using NeoServer.Server.Jobs.Channels;
using NeoServer.Server.Jobs.Creatures;
using NeoServer.Server.Jobs.Items;
using NeoServer.Server.Jobs.Persistance;
using NeoServer.Server.Security;
using NeoServer.Server.Standalone.IoC;
using NeoServer.Server.Tasks;
using Serilog;
using Serilog.Core;

namespace NeoServer.Server.Standalone
{
    public class Program
    {
        public static void Main()
        {
            Console.Title = "OpenCoreMMO Server";

            var stepSw = new Stopwatch();

            var sw = new Stopwatch();
            sw.Start();

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            var container = Container.CompositionRoot();

            var (serverConfiguration, gameConfiguration, logConfiguration) = (container.Resolve<ServerConfiguration>(),
                container.Resolve<GameConfiguration>(), container.Resolve<LogConfiguration>());
            var (logger, loggerConfiguration) = (container.Resolve<Logger>(), container.Resolve<LoggerConfiguration>());

            logger.Information("Welcome to OpenCoreMMO Server!");

            logger.Information("Log set to: {log}", logConfiguration.MinimumLevel);
            logger.Information("Environment: {env}", Environment.GetEnvironmentVariable("ENVIRONMENT"));

            logger.Step("Building extensions...", "Extensions builded",
                () => ScriptCompiler.Compile(serverConfiguration.Data, serverConfiguration.Extensions));

            var databaseConfiguration = container.Resolve<DatabaseConfiguration>();
            var context = container.Resolve<NeoContext>();

            logger.Step("Loading database: {db}", "{db} database loaded", () => context.Database.EnsureCreatedAsync(),
                databaseConfiguration.Active);

            RSA.LoadPem(serverConfiguration.Data);

            container.Resolve<IEnumerable<IRunBeforeLoaders>>().ToList().ForEach(x => x.Run());

            container.Resolve<ItemTypeLoader>().Load();

            container.Resolve<WorldLoader>().Load();

            container.Resolve<SpawnLoader>().Load();

            container.Resolve<MonsterLoader>().Load();
            container.Resolve<VocationLoader>().Load();
            container.Resolve<SpellLoader>().Load();

            container.Resolve<IEnumerable<IStartupLoader>>().ToList().ForEach(x => x.Load());

            container.Resolve<SpawnManager>().StartSpawn();

            var scheduler = container.Resolve<IScheduler>();
            var dispatcher = container.Resolve<IDispatcher>();

            dispatcher.Start(cancellationToken);
            scheduler.Start(cancellationToken);

            scheduler.AddEvent(new SchedulerEvent(1000, container.Resolve<GameCreatureJob>().StartChecking));
            scheduler.AddEvent(new SchedulerEvent(1000, container.Resolve<GameItemJob>().StartChecking));
            scheduler.AddEvent(new SchedulerEvent(1000, container.Resolve<GameChatChannelJob>().StartChecking));
            container.Resolve<PlayerPersistenceJob>().Start(cancellationToken);

            container.Resolve<EventSubscriber>().AttachEvents();
            container.Resolve<LuaGlobalRegister>().Register();

            var listeningTask = StartListening(container, cancellationToken);

            container.Resolve<IGameServer>().Open();

            sw.Stop();

            logger.Step("Running Garbage Collector", "Garbage collected", () =>
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            });

            logger.Information("Memory usage: {mem} MB",
                Math.Round(Process.GetCurrentProcess().WorkingSet64 / 1024f / 1024f, 2));

            logger.Information("Server is {up}! {time} ms", "up", sw.ElapsedMilliseconds);

            listeningTask.Wait(cancellationToken);
        }

        private static async Task StartListening(IContainer container, CancellationToken token)
        {
            container.Resolve<LoginListener>().BeginListening();
            container.Resolve<GameListener>().BeginListening();

            while (!token.IsCancellationRequested) await Task.Delay(TimeSpan.FromSeconds(1), token);
        }
    }
}