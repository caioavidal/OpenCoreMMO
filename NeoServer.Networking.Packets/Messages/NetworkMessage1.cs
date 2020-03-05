using NeoServer.Networking.Packets.Messages;

public class NetworkMessage
{
    public NetworkMessage Header { get; private set; }
    public NetworkMessage Body { get; private set; }
    public NetworkMessage()
    {
    }
}