using System;
using Microsoft.EntityFrameworkCore;

namespace NeoServer.Data.Providers.MySQL.Extensions
{
    public static class DbContextFactoryExtensions
    {
        public static DbContextOptions<NeoContext> UseMySql(this DbContextFactory factory, string name)
        {
            var builder = new DbContextOptionsBuilder<NeoContext>();
            builder.UseMySql(serverVersion: new MySqlServerVersion(new Version(5, 7, 17)), connectionString: name);

            return builder.Options;
        }
    }
}