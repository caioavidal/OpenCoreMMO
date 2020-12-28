using Autofac;
using NeoServer.Data;
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

//TEST EF
var context = container.Resolve<NeoContext>();

var account1 = new AccountModel
{
    Name = "1",
    Email = "1@gmail.com",
    Password = "1",
    PremiumTime = 1
};

var account2 = new AccountModel
{
    Name = "2",
    Email = "2@gmail.com",
    Password = "2",
    PremiumTime = 1
};

var player1 = new PlayerModel
{
    AccountId = 1,
    Name = "Developer1",
    ChaseMode = NeoServer.Game.Common.Players.ChaseMode.Follow,
    Capacity = 90000,
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

var player2 = new PlayerModel
{
    AccountId = 2,
    Name = "Developer2",
    ChaseMode = NeoServer.Game.Common.Players.ChaseMode.Follow,
    Capacity = 90000,
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
    PosY = 1022,
    PosZ = 7,
    StaminaMinutes = 2520
};

var playerItem1_Necklace = new PlayerItemModel
{
    PlayerId = 1,
    Pid  = 1,
    Sid = 101,
    Itemtype = 2125,
    Count = 1,
};

var playerItem1_Head = new PlayerItemModel
{
    PlayerId = 1,
    Pid = 2,
    Sid = 102,
    Itemtype = 2498,
    Count = 1,
};

var playerItem1_Backpack = new PlayerItemModel
{
    PlayerId = 1,
    Pid = 3,
    Sid = 103,
    Itemtype = 1988,
    Count = 1,
};

var playerItem1_Left = new PlayerItemModel
{
    PlayerId = 1,
    Pid = 4,
    Sid = 104,
    Itemtype = 2409,
    Count = 1,
};

var playerItem1_Body = new PlayerItemModel
{
    PlayerId = 1,
    Pid = 5,
    Sid = 105,
    Itemtype = 2466,
    Count = 1,
};

//var playerItem1_Right = new PlayerItemModel
//{
//    PlayerId = 1,
//    Pid = 6,
//    Sid = 1988,
//    Count = 1,
//};

var playerItem1_Ring = new PlayerItemModel
{
    PlayerId = 1,
    Pid = 7,
    Sid = 106,
    Itemtype = 6093,
    Count = 1,
};

var playerItem1_Legs = new PlayerItemModel
{
    PlayerId = 1,
    Pid = 8,
    Sid = 107,
    Itemtype = 2488,
    Count = 1,
};

var playerItem1_Ammo = new PlayerItemModel
{
    PlayerId = 1,
    Pid = 9,
    Sid = 108,
    Itemtype = 7840,
    Count = 1,
};

var playerItem1_Feet = new PlayerItemModel
{
    PlayerId = 1,
    Pid = 10,
    Sid = 109,
    Itemtype = 2666,
    Count = 1,
};

var playerItem1_b1 = new PlayerItemModel
{
    PlayerId = 1,
    Pid = 103,
    Sid = 110,
    Itemtype = 1988,
    Count = 1,
};

context.Accounts.Add(account1);
context.Accounts.Add(account2);
context.SaveChanges();

context.Player.Add(player1);
context.Player.Add(player2);
context.SaveChanges();

context.PlayerItems.Add(playerItem1_Necklace);
context.PlayerItems.Add(playerItem1_Head);
context.PlayerItems.Add(playerItem1_Backpack);
context.PlayerItems.Add(playerItem1_Left);
context.PlayerItems.Add(playerItem1_Body);
context.PlayerItems.Add(playerItem1_Ring);
context.PlayerItems.Add(playerItem1_Legs);
context.PlayerItems.Add(playerItem1_Ammo);
context.PlayerItems.Add(playerItem1_Feet);

context.PlayerItems.Add(playerItem1_b1);
context.SaveChanges();

//TEST EF

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


