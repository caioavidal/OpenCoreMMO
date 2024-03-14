using NeoServer.Game.Common;
using NeoServer.Server.Configurations;
using Serilog;
using System.Diagnostics;
using System.Net;
using NeoServer.Server.Standalone.IoC;
using Autofac;
using ILogger = Serilog.ILogger;
using NeoServer.Server.Helpers.Extensions;
using NeoServer.Server.Compiler;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.World.Models.Spawns;
using NeoServer.Loaders.Interfaces;
using NeoServer.Loaders.Items;
using NeoServer.Loaders.Monsters;
using NeoServer.Loaders.Quest;
using NeoServer.Loaders.Spawns;
using NeoServer.Loaders.Spells;
using NeoServer.Loaders.Vocations;
using NeoServer.Loaders.World;
using NeoServer.Scripts.Lua;
using NeoServer.Server.Common.Contracts.Tasks;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Events.Subscribers;
using NeoServer.Server.Jobs.Channels;
using NeoServer.Server.Jobs.Creatures;
using NeoServer.Server.Jobs.Items;
using NeoServer.Server.Jobs.Persistence;
using NeoServer.Server.Tasks;
using IStartup = NeoServer.Server.Common.Contracts.IStartup;
using NeoServer.Data.Contexts;
using NeoServer.Networking.Listeners;
using NeoServer.Server.Security;
using System.Net.Sockets;
using NeoServer.Networking.Packets.Connection;
using NeoServer.Server.Common.Contracts.Network;
using System.Net.WebSockets;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Connection = NeoServer.Networking.Packets.Connection.Connection;
using System.Collections.Generic;
using NeoServer.Networking.Handlers;
using System;
using NeoServer.Networking.Packets.Security;
using NeoServer.Networking.Protocols;
using Microsoft.AspNetCore.Server.IIS;

namespace NeoServer.Server.WS
{
    public class Program
    {
        private static IDictionary<WebSocket, IConnection> clientMap; 
        private static Func<IConnection, IPacketHandler> _handlerFactory;
        private static GameProtocol _gameProtocol;

        public static async Task Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.ConfigureLogging(l =>
            {
                l.AddConsole();

            });
            builder.WebHost.UseIISIntegration();
            builder.WebHost.UseIIS();
            var app = builder.Build();

            app.UseWebSockets();
            app.UseDeveloperExceptionPage();


            #region Neo

            var sw = new Stopwatch();
            sw.Start();

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            var container = Container.BuildConfigurations();

            var (serverConfiguration, _, logConfiguration) = (container.Resolve<ServerConfiguration>(),
                container.Resolve<GameConfiguration>(), container.Resolve<LogConfiguration>());

            var (logger, _) = (container.Resolve<ILogger>(), container.Resolve<LoggerConfiguration>());

            logger.Information("Welcome to OpenCoreMMO Server!");

            logger.Information("Log set to: {Log}", logConfiguration.MinimumLevel);
            logger.Information("Environment: {Env}", Environment.GetEnvironmentVariable("ENVIRONMENT"));

            logger.Step("Building extensions...", "{files} extensions build",
                () => ExtensionsCompiler.Compile(serverConfiguration.Data, serverConfiguration.Extensions));

            container = Container.BuildAll();
            Helpers.IoC.Initialize(container);

            GameAssemblyCache.Load();

            await LoadDatabase(container, logger, cancellationToken);

            Rsa.LoadPem(serverConfiguration.Data);

            container.Resolve<IEnumerable<IRunBeforeLoaders>>().ToList().ForEach(x => x.Run());
            container.Resolve<FactoryEventSubscriber>().AttachEvents();

            container.Resolve<ItemTypeLoader>().Load();

            container.Resolve<QuestDataLoader>().Load();

            container.Resolve<WorldLoader>().Load();

            container.Resolve<SpawnLoader>().Load();

            container.Resolve<MonsterLoader>().Load();
            container.Resolve<VocationLoader>().Load();
            container.Resolve<SpellLoader>().Load();

            container.Resolve<IEnumerable<IStartupLoader>>().ToList().ForEach(x => x.Load());

            container.Resolve<SpawnManager>().StartSpawn();

            var scheduler = container.Resolve<IScheduler>();
            var dispatcher = container.Resolve<IDispatcher>();
            var persistenceDispatcher = container.Resolve<IPersistenceDispatcher>();

            dispatcher.Start(cancellationToken);
            scheduler.Start(cancellationToken);
            persistenceDispatcher.Start(cancellationToken);

            scheduler.AddEvent(new SchedulerEvent(1000, container.Resolve<GameCreatureJob>().StartChecking));
            scheduler.AddEvent(new SchedulerEvent(1000, container.Resolve<GameItemJob>().StartChecking));
            scheduler.AddEvent(new SchedulerEvent(1000, container.Resolve<GameChatChannelJob>().StartChecking));
            container.Resolve<PlayerPersistenceJob>().Start(cancellationToken);

            container.Resolve<EventSubscriber>().AttachEvents();
            container.Resolve<IEnumerable<IStartup>>().ToList().ForEach(x => x.Run());

            container.Resolve<LuaGlobalRegister>().Register();

            StartListening(container, cancellationToken);

            container.Resolve<IGameServer>().Open();

            sw.Stop();

            logger.Step("Running Garbage Collector", "Garbage collected", () =>
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            });

            logger.Information("Memory usage: {Mem} MB",
                Math.Round(Process.GetCurrentProcess().WorkingSet64 / 1024f / 1024f, 2));

            logger.Information("Server is {Up}! {Time} ms", "up", sw.ElapsedMilliseconds);

            #endregion

            clientMap = new Dictionary<WebSocket, IConnection>();

            //_handlerFactory = app.Services.GetService<Func<IConnection, IPacketHandler>>();
            //_gameProtocol = container.Resolve< Func < IConnection, IPacketHandler >> ();
            _handlerFactory = container.Resolve< Func < IConnection, IPacketHandler >> ();

            app.Map("/", async context =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var ws = await context.WebSockets.AcceptWebSocketAsync();

                    var connection = new Connection(ws, logger);

                    Debug.WriteLine("IsWebSocketRequest");
                    clientMap.Add(ws, connection);

                    await ReceiveMessage(connection, OnMessageReceived);
                }
                else
                {
                    Debug.WriteLine("!IsWebSocketRequest");
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            });

            await app.RunAsync(cancellationToken);
        }


        private static async Task LoadDatabase(IComponentContext container, ILogger logger,
            CancellationToken cancellationToken)
        {
            var (_, databaseName) = container.Resolve<DatabaseConfiguration>();
            var context = container.Resolve<NeoContext>();

            logger.Information("Loading database: {Db}", databaseName);

            try
            {
                await context.Database.EnsureCreatedAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Unable to connect to database");
                Environment.Exit(0);
            }

            logger.Information("{Db} database loaded", databaseName);
        }

        private static void StartListening(IComponentContext container, CancellationToken cancellationToken)
        {
            container.Resolve<LoginListener>().BeginListening(cancellationToken);
            container.Resolve<GameListener>().BeginListening(cancellationToken);
        }

        private static async Task ReceiveMessage(Connection connection, Action<Connection, WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[16394];
            while (connection.WebSocket.State == WebSocketState.Open)
            {
                var result = await connection.WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                //socket.stre
                handleMessage(connection, result, buffer);
            }
        }

        private static async void OnMessageReceived(Connection connection, WebSocketReceiveResult result, byte[] buffer)
        {
            //var inboundMessage = new NetworkMessage(isOutbound: false);
            //var isAuthenticated = true;
            //var packetDataWriter = new NetDataWriter();

            if (result.MessageType == WebSocketMessageType.Binary)
            {
                connection.ReadWebSocket(buffer);

                //_gameProtocol.HandleMessage(connection);

                //if (connection.IsAuthenticated && !connection.Disconnected)
                //    Xtea.Decrypt(connection.InMessage, 6, connection.XteaKey);

                if (_handlerFactory(connection) is not { } handler) return;

                handler?.HandleMessage(connection.InMessage, connection);
            }
            else if (result.MessageType == WebSocketMessageType.Close || connection.WebSocket.State == WebSocketState.Aborted)
            {
                //game.LogPlayerOut(client.PlayerId);
                clientMap.Remove(connection.WebSocket);
                await connection.WebSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            }
        }
    }
}
