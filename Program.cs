using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace neoserver
{
    class Program
    {
        static void Main(string[] args)
        {
            AsynchronousSocketListener.StartListening();

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
