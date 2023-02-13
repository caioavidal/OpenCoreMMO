using Microsoft.EntityFrameworkCore;
using NeoServer.Data.Contexts;
using NeoServer.Data.Factory;

namespace NeoServer.Data.Providers.InMemory;

public static class DbContextFactoryExtensions
{
    public static DbContextOptions<NeoContext> UseInMemory(this DbContextFactory factory, string name)
    {
        var builder = new DbContextOptionsBuilder<NeoContext>();
        builder.UseInMemoryDatabase(name);

        return builder.Options;
    }
}