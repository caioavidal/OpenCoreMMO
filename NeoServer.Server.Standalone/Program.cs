using Autofac;
using NeoServer.Data.RavenDB;
using NeoServer.Networking.Listeners;
using NeoServer.Server.Contracts.Repositories;
using NeoServer.Server.Loaders;
using NeoServer.Server.Model;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Security;
using NeoServer.Server.Standalone.IoC;
using NeoServer.Server.World;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeoServer.Server.Standalone
{
    class Program
    {
        static void Main(string[] args)
        {

            var container = Container.CompositionRoot();
            container.Resolve<Database>().Connect();

            //CreateChar();

           RSA.LoadPem();

            ItemLoader.Load();

            container.Resolve<IWorldLoader>().Load();

            container.Resolve<World.Map.Map>();

            Task.Run(() => container.Resolve<LoginListener>().BeginListening());
            Task.Run(() => container.Resolve<GameListener>().BeginListening());

            Console.WriteLine("NeoServer is up!");
            Console.Read();
        }

        public static void CreateChar()
        {
            var a = new Account
            {
                AccountName = "1",
                Password = "1"
            };
            a.Players = new List<Player>(){
                     new Player(){
                          CharacterName = "Caio",
                       
                            ChaseMode = ChaseMode.Follow,
                             Gender = Gender.Male,
                              MaxSoulPoints = 100,
                               Online = false,
                                 SoulPoints = 100,
                                 
                                  Vocation = VocationType.Knight

                     }
                  };
            Container.CompositionRoot().Resolve<IAccountRepository>().Create(a);
        }
    }
}
