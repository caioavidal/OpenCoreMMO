using System.Collections.Generic;

namespace NeoServer.Server.Standalone
{
    public enum DatabaseType
    {
        INMEMORY,
        MONGODB,
        MYSQL,
        MSSQL,
        SQLITE
    }

    public record ServerConfiguration(int Version, string OTBM, string OTB, string Data, string ServerName);
    public record LogConfiguration(string MinimumLevel);


    public record DatabaseConfiguration(string InMemory, string MongoDB, string SQLite, string MySQL, string MSSQL, string active);
    public record DatabaseConfiguration2(Dictionary<DatabaseType, string> connections, DatabaseType active);
}
