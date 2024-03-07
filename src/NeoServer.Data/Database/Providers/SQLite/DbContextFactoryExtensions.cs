using Microsoft.EntityFrameworkCore;
using NeoServer.Infrastructure.Database.Contexts;
using NeoServer.Infrastructure.Database.Factory;

namespace NeoServer.Infrastructure.Database.Providers.SQLite;

public static class DbContextFactoryExtensions
{
    public static DbContextOptions<NeoContext> UseSQLite(this DbContextFactory factory, string name)
    {
        var builder = new DbContextOptionsBuilder<NeoContext>();
        builder.UseSqlite(name);

        return builder.Options;
    }
}