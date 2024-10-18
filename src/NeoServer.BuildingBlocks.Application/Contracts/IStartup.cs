namespace NeoServer.BuildingBlocks.Application.Contracts;

public interface IStartup
{
    void Run();
}

public interface IRunBeforeLoaders
{
    void Run();
}