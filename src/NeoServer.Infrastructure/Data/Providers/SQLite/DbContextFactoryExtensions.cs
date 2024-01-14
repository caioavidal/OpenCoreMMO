using Microsoft.EntityFrameworkCore;
using NeoServer.Infrastructure.Data.Contexts;
using NeoServer.Infrastructure.Data.Factory;

namespace NeoServer.Infrastructure.Data.Providers.SQLite;

public static class DbContextFactoryExtensions
{
    public static DbContextOptions<NeoContext> UseSQLite(this DbContextFactory factory, string name)
    {
        var builder = new DbContextOptionsBuilder<NeoContext>();
        builder.UseSqlite(name);

        return builder.Options;
    }
}