using Autofac;
using NeoServer.Data.Model;
using NeoServer.Data.RavenDB;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creature.Model;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Players;
using NeoServer.Game.Items;
using NeoServer.Loaders.Items;
using NeoServer.Loaders.World;
using NeoServer.Networking.Listeners;
using NeoServer.OTBM;
using NeoServer.Server.Contracts.Repositories;

using NeoServer.Server.Model.Players;
using NeoServer.Server.Schedulers;
using NeoServer.Server.Schedulers.Contracts;
using NeoServer.Server.Security;
using NeoServer.Server.Standalone.IoC;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace NeoServer.Server.Standalone
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            var container = Container.CompositionRoot();
            container.Resolve<Database>().Connect();

            var logger = container.Resolve<Logger>();

            RSA.LoadPem();

            container.Resolve<ItemTypeLoader>().Load();

            container.Resolve<WorldLoader>().Load();

            var listeningTask = StartListening(container, cancellationToken);

            var scheduler = container.Resolve<IScheduler>();

            Task.Factory.StartNew(() => scheduler.Start(cancellationToken), TaskCreationOptions.LongRunning);


            container.Resolve<ServerState>().OpenServer();


            sw.Stop();
            logger.Information($"Server is up! Took {sw.ElapsedMilliseconds} Milliseconds");


            listeningTask.Wait(cancellationToken);

            //   CreateChar();
        }

        private static async Task StartListening(IContainer container, CancellationToken token)
        {
            container.Resolve<LoginListener>().BeginListening();
            container.Resolve<GameListener>().BeginListening();

            while (!token.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), token);
            }
        }

        public static void CreateChar()
        {
            var a = new AccountModel
            {
                AccountName = "1",
                Password = "1"
            };

            a.Players = new List<PlayerModel>(){
                     new PlayerModel(){
                        CharacterName = "Caio",
                        Capacity = 100,
                        HealthPoints = 100,
                        Mana = 100,
                        MaxHealthPoints = 100,
                        MaxMana = 100,
                        StaminaMinutes = 2520,
                        ChaseMode = ChaseMode.Follow,
                        Gender = Gender.Male,
                        MaxSoulPoints = 100,
                        Online = false,
                        // Location = new Location{
                        //     X = 160,
                        //     Y = 54,
                        //     Z = 7
                        // },
                        SoulPoints = 100,
                        Vocation = VocationType.Knight,
                        Outfit = new Outfit
                        {
                            LookType = 75,
                            Addon = 0,
                            Head = 0,
                            Body = 0,
                            Legs = 0,
                            Feet = 0
                        },
                        Skills = new Dictionary<SkillType, ISkill>
                        {
                            { SkillType.Level, new Skill(SkillType.Level,10, 1.0, 10, 10, 150) },
                            { SkillType.Magic , new Skill(SkillType.Magic, 1, 1.0, 10, 1, 150)},
                            { SkillType.Fist, new Skill(SkillType.Fist, 10, 1.0, 10, 10, 150)},
                            { SkillType.Axe, new Skill(SkillType.Axe, 10, 1.0, 10, 10, 150)},
                            { SkillType.Club, new Skill(SkillType.Club, 10, 1.0, 10, 10, 150)},
                            { SkillType.Sword, new Skill(SkillType.Sword, 10, 1.0, 10, 10, 150)},
                            { SkillType.Shield, new Skill(SkillType.Shield, 10, 1.0, 10, 10, 150)},
                            { SkillType.Distance, new Skill(SkillType.Distance, 10, 1.0, 10, 10, 150)},
                            { SkillType.Fishing, new Skill(SkillType.Fishing, 10, 1.0, 10, 10, 150)}
                        },
                        Inventory = new Dictionary<Slot, ushort>
                        {
                            { Slot.Backpack, 2854 }
                        },
                        Speed = 800

                     }
                  };

            Container.CompositionRoot().Resolve<IAccountRepository>().Create(a);
        }
    }
}
