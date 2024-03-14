namespace NeoServer.Networking.Packets.Network;

public interface IConnectionEventArgs
{
    IConnection Connection { get; }
}