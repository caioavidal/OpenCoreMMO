using Microsoft.EntityFrameworkCore;
using NeoServer.Infrastructure.Data.Contexts;
using NeoServer.Infrastructure.Data.Factory;

namespace NeoServer.Infrastructure.Data.Providers.InMemory;

public static class DbContextFactoryExtensions
{
    public static DbContextOptions<NeoContext> UseInMemory(this DbContextFactory factory, string name)
    {
        var builder = new DbContextOptionsBuilder<NeoContext>();
        builder.UseInMemoryDatabase(name);

        return builder.Options;
    }
}