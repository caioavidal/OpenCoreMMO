using Autofac;
using NeoServer.Data.RavenDB;
using NeoServer.Networking.Listeners;
using NeoServer.Server.Contracts.Repositories;
using NeoServer.Server.Model;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Security;
using NeoServer.Server.Standalone.IoC;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeoServer.Server.Standalone
{
    class Program
    {
        static void Main(string[] args)
        {


            
            Console.WriteLine(Convert.ToString(c, toBase: 2));

            return;

            var container = Container.CompositionRoot();
            container.Resolve<Database>().Connect();

            // Container.CompositionRoot().Resolve<IAccountRepository>().Create(new Account
            // {
            //     AccountName = "1",
            //     Password = "1",
            //     Players = new List<Player>(){
            //         new Player(){
            //              Name = "Caio 1"
            //         }, new Player(){
            //             Name = "Caio 2"
            //         }
            //      }
            //  });

            RSA.LoadPem();

            Task.Run(() => container.Resolve<LoginListener>().BeginListening());
            Task.Run(() => container.Resolve<GameListener>().BeginListening());
            //new LoginListener().BeginListening();
            Console.WriteLine("NeoServer is up!");
            Console.Read();
        }
    }
}
