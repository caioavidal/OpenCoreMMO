using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace neoserver
{
    class Program
    {
        static void Main(string[] args)
        {
            MessageQueue.Start();

            IoC.Load();
            RSA.LoadPem();

            Database.Connect();

            VocationConfig.Load();

            var loadItemsTask = OtbFile.LoadItems();

            ItemConfig.Load();

            AsynchronousSocketListener.StartListening();


            Console.Read();

            //         var builder = new ConfigurationBuilder()
            //   .SetBasePath(Directory.GetCurrentDirectory())
            //   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);


            //         IConfigurationRoot configuration = builder.Build();

            //         var serviceProvider = new ServiceCollection()
            //         .Configure<GameConfiguration>(configuration.GetSection("Game"));




            //         serviceProvider.BuildServiceProvider();

        }
    }
}
