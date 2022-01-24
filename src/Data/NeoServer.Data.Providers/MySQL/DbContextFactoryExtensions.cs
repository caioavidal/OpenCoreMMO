using System;
using Microsoft.EntityFrameworkCore;
using NeoServer.Data.Contexts;
using NeoServer.Data.Factory;

namespace NeoServer.Data.Providers.MySQL;

public static class DbContextFactoryExtensions
{
    public static DbContextOptions<NeoContext> UseMySql(this DbContextFactory factory, string name)
    {
        var builder = new DbContextOptionsBuilder<NeoContext>();
        builder.UseMySql(serverVersion: new MySqlServerVersion(new Version(5, 7, 17)), connectionString: name);

        return builder.Options;
    }
}