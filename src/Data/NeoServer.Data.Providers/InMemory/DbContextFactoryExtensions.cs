using Microsoft.EntityFrameworkCore;

namespace NeoServer.Data.Providers.InMemoryDB.Extensions
{
    public static class DbContextFactoryExtensions
    {
        public static DbContextOptions<NeoContext> UseInMemory(this DbContextFactory factory, string name)
        {
            var builder = new DbContextOptionsBuilder<NeoContext>();
            builder.UseInMemoryDatabase(name);

            return builder.Options;
        }
    }
}