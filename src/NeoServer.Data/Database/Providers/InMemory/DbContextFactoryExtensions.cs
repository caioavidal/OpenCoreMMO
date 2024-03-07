using Microsoft.EntityFrameworkCore;
using NeoServer.Infrastructure.Database.Contexts;
using NeoServer.Infrastructure.Database.Factory;

namespace NeoServer.Infrastructure.Database.Providers.InMemory;

public static class DbContextFactoryExtensions
{
    public static DbContextOptions<NeoContext> UseInMemory(this DbContextFactory factory, string name)
    {
        var builder = new DbContextOptionsBuilder<NeoContext>();
        builder.UseInMemoryDatabase(name);

        return builder.Options;
    }
}