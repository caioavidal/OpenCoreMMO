using Autofac;
using NeoServer.Data;
using NeoServer.Data.RavenDB;
using NeoServer.Game.World.Spawns;
using NeoServer.Loaders.Items;
using NeoServer.Loaders.Monsters;
using NeoServer.Loaders.Spawns;
using NeoServer.Loaders.World;
using NeoServer.Networking.Listeners;
using NeoServer.Server.Events;
using NeoServer.Server.Jobs.Creatures;
using NeoServer.Server.Jobs.Items;
using NeoServer.Server.Security;
using NeoServer.Server.Standalone.IoC;
using NeoServer.Server.Tasks;
using NeoServer.Server.Contracts.Tasks;
using Serilog.Core;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NeoServer.Server;
using NeoServer.Loaders.Vocations;
using NeoServer.Data.Model;
using NeoServer.Server.Model.Players;
using NeoServer.Data.Interfaces;

Console.Title = "OpenCoreMMO Server";

var sw = new Stopwatch();
sw.Start();

var cancellationTokenSource = new CancellationTokenSource();
var cancellationToken = cancellationTokenSource.Token;

var container = Container.CompositionRoot();
container.Resolve<Database>().Connect();

var context = container.Resolve<NeoContext>();

var account = new AccountModel
{
    Name = "1",
    Email = "1@gmail.com",
    Password = "1",
    PremiumTime = 1
};

var player = new PlayerModel
{
    AccountId = 1,
    Name = "Developer",
    ChaseMode = NeoServer.Game.Common.Players.ChaseMode.Follow,
    Level = 100,
    Health = 4440,
    MaxHealth = 4440,
    Vocation = NeoServer.Game.Common.Players.VocationType.Knight,
    Gender = NeoServer.Game.Common.Players.Gender.Male,
    Speed = 800,
    Online = false,
    Mana = 1750,
    MaxMana = 1750,
    Soul = 100,
    MaxSoul = 100,
    PosX = 1020,
    PosY = 1023, 
    PosZ = 7,
    StaminaMinutes = 2520
};

context.Accounts.Add(account);

context.SaveChanges();

var accounts = context.Accounts.AsQueryable().ToList();

context.Player.Add(player);

context.SaveChanges();

var players = context.Player.AsQueryable().ToList();

var repo = container.Resolve<IAccountRepositoryNeo>();

var acc = repo.GetAllAsync().Result;

var logger = container.Resolve<Logger>();

RSA.LoadPem();

//   ScriptCompiler.Compile();

container.Resolve<ItemTypeLoader>().Load();

container.Resolve<WorldLoader>().Load();

container.Resolve<SpawnLoader>().Load();

container.Resolve<MonsterLoader>().Load();
container.Resolve<VocationLoader>().Load();
//new SpellLoader().Load();
container.Resolve<SpawnManager>().StartSpawn();

var listeningTask = StartListening(container, cancellationToken);

var scheduler = container.Resolve<IScheduler>();
var dispatcher = container.Resolve<IDispatcher>();

dispatcher.Start(cancellationToken);
scheduler.Start(cancellationToken);

scheduler.AddEvent(new SchedulerEvent(1000, container.Resolve<GameCreatureJob>().StartCheckingCreatures));
scheduler.AddEvent(new SchedulerEvent(1000, container.Resolve<GameItemJob>().StartCheckingItems));

container.Resolve<EventSubscriber>().AttachEvents();

container.Resolve<Game>().Open();

sw.Stop();

logger.Information($"Running Garbage Collector");

GC.Collect();
GC.WaitForPendingFinalizers();

logger.Information($"Server is up! {sw.ElapsedMilliseconds} ms");

logger.Information($"Memory usage: {Math.Round((System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / 1024f) / 1024f, 2)} MB");

listeningTask.Wait(cancellationToken);

static async Task StartListening(IContainer container, CancellationToken token)
{
    container.Resolve<LoginListener>().BeginListening();
    container.Resolve<GameListener>().BeginListening();

    while (!token.IsCancellationRequested)
    {
        await Task.Delay(TimeSpan.FromSeconds(1), token);
    }
}


