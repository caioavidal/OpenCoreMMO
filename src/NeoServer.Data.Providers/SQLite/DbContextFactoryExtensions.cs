using Microsoft.EntityFrameworkCore;

namespace NeoServer.Data.Providers.SQLite.Extensions
{
    public static partial class DbContextFactoryExtensions
    {
        public static DbContextOptions<NeoContext> UseSQLite(this DbContextFactory factory, string name)
        {
            var builder = new DbContextOptionsBuilder<NeoContext>();
            builder.UseSqlite(name);

             
            return builder.Options;
        }
    }
}
