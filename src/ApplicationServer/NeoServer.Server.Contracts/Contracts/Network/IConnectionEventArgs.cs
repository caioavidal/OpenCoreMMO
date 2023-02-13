namespace NeoServer.Server.Common.Contracts.Network;

public interface IConnectionEventArgs
{
    IConnection Connection { get; }
}