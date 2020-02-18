using NeoServer.Networking.Listeners;
using System;

namespace NeoServer.Server.Standalone
{
    class Program
    {
        static void Main(string[] args)
        {
            new LoginListener().BeginListening();

            Console.Read();
        }
    }
}
