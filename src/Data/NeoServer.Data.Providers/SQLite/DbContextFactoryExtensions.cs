using Microsoft.EntityFrameworkCore;
using NeoServer.Data.Contexts;
using NeoServer.Data.Factory;

namespace NeoServer.Data.Providers.SQLite;

public static class DbContextFactoryExtensions
{
    public static DbContextOptions<NeoContext> UseSQLite(this DbContextFactory factory, string name)
    {
        var builder = new DbContextOptionsBuilder<NeoContext>();
        builder.UseSqlite(name);

        return builder.Options;
    }
}