using Autofac;
using NeoServer.Networking.Listeners;
using NeoServer.Server.Security;
using NeoServer.Server.Standalone.IoC;
using System;

namespace NeoServer.Server.Standalone
{
    class Program
    {
        static void Main(string[] args)
        {
            RSA.LoadPem();
            Container.CompositionRoot().Resolve<LoginListener>().BeginListening();
            //new LoginListener().BeginListening();

            Console.Read();
        }
    }
}
