using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NeoServer.Application.Configurations;
using NeoServer.Data.Contexts;
using NeoServer.Data.Factory;
using NeoServer.Data.Interfaces;
using NeoServer.Data.Providers.InMemory;
using NeoServer.Data.Providers.MySQL;
using NeoServer.Data.Providers.SQLite;
using NeoServer.Data.Repositories;
using NeoServer.Data.Repositories.Player;
using Serilog;

namespace NeoServer.Shared.IoC.Modules;

public static class DatabaseInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection builder)
    {
        builder.AddSingleton<IAccountRepository, AccountRepository>();
        builder.AddSingleton<IGuildRepository, GuildRepository>();
        builder.AddSingleton<IPlayerDepotItemRepository, PlayerDepotItemRepository>();
        builder.AddSingleton<IPlayerRepository, PlayerRepository>();
        builder.AddSingleton(typeof(BaseRepository<>));

        return builder;
    }

    public static IServiceCollection AddDatabases(this IServiceCollection builder, IConfiguration configuration)
    {
        return builder.RegisterContext<NeoContext>(configuration);
    }

    private static IServiceCollection RegisterContext<TContext>(this IServiceCollection builder, IConfiguration configuration)
        where TContext : DbContext
    {
        DatabaseConfiguration config = new(null, DatabaseType.INMEMORY);

        configuration.GetSection("database").Bind(config);

        var options = config.Active switch
        {
            DatabaseType.INMEMORY => DbContextFactory.GetInstance()
                .UseInMemory(config.Connections[DatabaseType.INMEMORY]),
            DatabaseType.MONGODB => DbContextFactory.GetInstance()
                .UseInMemory(config.Connections[DatabaseType.MONGODB]),
            DatabaseType.MYSQL => DbContextFactory.GetInstance().UseMySql(config.Connections[DatabaseType.MYSQL]),
            DatabaseType.MSSQL => DbContextFactory.GetInstance()
                .UseInMemory(config.Connections[DatabaseType.MSSQL]),
            DatabaseType.SQLITE => DbContextFactory.GetInstance()
                .UseSQLite(config.Connections[DatabaseType.SQLITE]),
            _ => throw new ArgumentException("Invalid active database!")
        };

        builder.AddSingleton(config);
        builder.AddSingleton(options);


        builder.AddSingleton(x => new NeoContext(options, x.GetRequiredService<ILogger>()) as TContext);
            // .WithParameter("options", options)
            // .InstancePerLifetimeScope();

        return builder;
    }
}