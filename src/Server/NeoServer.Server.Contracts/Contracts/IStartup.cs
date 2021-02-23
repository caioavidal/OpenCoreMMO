namespace NeoServer.Server.Contracts
{
    public interface IStartup
    {
        void Run();
    }
    public interface IRunBeforeLoaders
    {
        void Run();
    }
}
