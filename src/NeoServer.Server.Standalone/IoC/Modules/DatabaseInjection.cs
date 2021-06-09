using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NeoServer.Data;
using NeoServer.Data.Interfaces;
using NeoServer.Data.Providers.InMemoryDB.Extensions;
using NeoServer.Data.Providers.MySQL.Extensions;
using NeoServer.Data.Providers.SQLite.Extensions;
using NeoServer.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Server.Standalone.IoC
{
    public static class DatabaseInjection
    {
        public static ContainerBuilder AddRepositories(this ContainerBuilder builder)
        {
            builder.RegisterType<AccountRepository>().As<IAccountRepository>().SingleInstance();
            builder.RegisterType<GuildRepository>().As<IGuildRepository>().SingleInstance();
            builder.RegisterType<PlayerDepotItemRepositoryNeo>().As<IPlayerDepotItemRepositoryNeo>().SingleInstance();
            return builder;
        }
        public static ContainerBuilder AddDatabases(this ContainerBuilder builder, IConfigurationRoot configuration)
        {
            builder.RegisterContext<NeoContext>(configuration);
            return builder;
        }

        private static void RegisterContext<TContext>(this ContainerBuilder builder, IConfigurationRoot configuration) where TContext : DbContext
        {
            DatabaseConfiguration2 config = new(null, DatabaseType.INMEMORY);

            configuration.GetSection("database").Bind(config);

            DbContextOptions<NeoContext> options = config.active switch
            {
                DatabaseType.INMEMORY => DbContextFactory.GetInstance().UseInMemory(config.connections[DatabaseType.INMEMORY]),
                DatabaseType.MONGODB => DbContextFactory.GetInstance().UseInMemory(config.connections[DatabaseType.MONGODB]),
                DatabaseType.MYSQL => DbContextFactory.GetInstance().UseMySql(config.connections[DatabaseType.MYSQL]),
                DatabaseType.MSSQL => DbContextFactory.GetInstance().UseInMemory(config.connections[DatabaseType.MSSQL]),
                DatabaseType.SQLITE => DbContextFactory.GetInstance().UseSQLite(config.connections[DatabaseType.SQLITE]),
                _ => throw new ArgumentException("Invalid active database!"),
            };

            builder.RegisterInstance(config).SingleInstance();

            builder.RegisterType<TContext>()
                   .WithParameter("options", options)
                   .InstancePerLifetimeScope();
        }
    }
}
