namespace NeoServer.Server.Common.Contracts;

public interface IStartup
{
    void Run();
}

public interface IRunBeforeLoaders
{
    void Run();
}