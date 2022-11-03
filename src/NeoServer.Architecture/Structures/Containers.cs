using C4Sharp.Models;
using C4Sharp.Models.Relationships;

namespace NeoServer.Architecture.Structures;

public static class Containers
{
    private static Container? _server;

    public static Container GameServer => _server ??= new Container(
        "NeoServer.Game", "NeoServer.Game")
    {
        ContainerType = ContainerType.ServerConsole,
        Description = "Process messages of client and reply it.",
        Technology = "C#",
        Boundary = Boundary.Internal
    };

    private static Container? _networkingServer;

    public static Container NetworkingServer => _networkingServer ??= new Container(
        "NeoServer.Networking", "NeoServer.Networking")
    {
        ContainerType = ContainerType.ServerConsole,
        Description = "Create network connection with the client bidirectional.",
        Technology = "C#",
        Boundary = Boundary.Internal,
    };

    private static Container? _jobsServer;

    public static Container JobsServer => _jobsServer ??= new Container(
        "NeoServer.Jobs", "NeoServer.Jobs")
    {
        ContainerType = ContainerType.ServerConsole,
        Description = "Routines for updates stats of player on database or push notifications",
        Technology = "C#",
        Boundary = Boundary.Internal,
    };


    private static Container? _loaderServer;

    public static Container LoadServer => _loaderServer ??= new Container(
        "NeoServer.Loaders", "NeoServer.Loaders")
    {
        ContainerType = ContainerType.ServerConsole,
        Description = "Load basic information of server like as Maps, Monsters, Itens, Missions and etc.",
        Technology = "C#",
        Boundary = Boundary.Internal,
    };

    private static Container? _fileSystems;

    public static Container FileSystemToLoader => _fileSystems ??= new Container(
        "NeoServer.Files", "NeoServer.Files")
    {
        ContainerType = ContainerType.FileSystem,
        Description = "Information of server like as Maps, Monsters, Itens, Missions and etc.",
        Technology = "C#",
        Boundary = Boundary.Internal,
    };


    private static Container? _sqlDatabase;

    public static Container SqlDatabase => _sqlDatabase ??= new Container(
        "SQL", "NeoServer.Data.SQL")
    {
        ContainerType = ContainerType.Database,
        Description = "Stores user registration information, hashed auth credentials, access logs, etc.",
        Technology = "Mysql Database",
        Boundary = Boundary.Internal
    };

    private static Container? _memoryDatabase;

    public static Container MemoryDatabase => _memoryDatabase ??= new Container(
        "Memory", "NeoServer.Data.Memory")
    {
        ContainerType = ContainerType.Database,
        Description = "Stores user registration information, hashed auth credentials, access logs, etc.",
        Technology = "Mysql Database",
        Boundary = Boundary.Internal
    };
}
