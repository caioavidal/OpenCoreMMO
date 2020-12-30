using Microsoft.EntityFrameworkCore;
using System;

namespace NeoServer.Data.Providers.MySQL.Extensions
{
    public static partial class DbContextFactoryExtensions
    {
        public static DbContextOptions<NeoContext> UseMySql(this DbContextFactory factory, string name)
        {
            var builder = new DbContextOptionsBuilder<NeoContext>();
            builder.UseMySql(serverVersion: new MySqlServerVersion(new Version(5, 7, 17)), connectionString: name);
            
            return builder.Options;
        }
    }
}
