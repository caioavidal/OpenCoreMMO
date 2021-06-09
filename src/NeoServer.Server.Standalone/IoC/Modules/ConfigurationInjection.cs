using System;
using System.IO;
using Autofac;
using Microsoft.Extensions.Configuration;
using NeoServer.Game.Common;

namespace NeoServer.Server.Standalone.IoC
{
    public static class ConfigurationInjection
    {
        public static IConfigurationRoot GetConfiguration()
        {
            var environmentName = Environment.GetEnvironmentVariable("ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                .AddEnvironmentVariables(); //.Build();

            //only add secrets in development
            if (environmentName.Equals("Local", StringComparison.InvariantCultureIgnoreCase))
                builder.AddUserSecrets<Program>();
            return builder.Build();
        }

        public static ContainerBuilder AddConfigurations(this ContainerBuilder builder,
            IConfigurationRoot configuration)
        {
            ServerConfiguration serverConfiguration =
                new(0, null, null, null, string.Empty, string.Empty, new SaveConfiguration(3600));
            GameConfiguration gameConfiguration = new();
            LogConfiguration logConfiguration = new(null);

            configuration.GetSection("server").Bind(serverConfiguration);
            configuration.GetSection("game").Bind(gameConfiguration);
            configuration.GetSection("log").Bind(logConfiguration);

            builder.RegisterInstance(serverConfiguration).SingleInstance();
            builder.RegisterInstance(gameConfiguration).SingleInstance();
            builder.RegisterInstance(logConfiguration).SingleInstance();

            return builder;
            //return (serverConfiguration, gameConfiguration, logConfiguration);
        }
    }
}