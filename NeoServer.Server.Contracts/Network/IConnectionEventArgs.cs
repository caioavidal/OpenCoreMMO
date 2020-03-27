namespace NeoServer.Server.Contracts.Network
{
    public interface IConnectionEventArgs
    {
        IConnection Connection { get; }
    }
}