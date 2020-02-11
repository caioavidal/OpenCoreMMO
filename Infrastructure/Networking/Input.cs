using System.Net.Sockets;

public abstract class Input
{
    public byte[] Buffer { get; private set; }
    public Socket Socket { get; private set; }
    public uint[] Xtea { get; private set; } = new uint[4];

    public Input(byte[] buffer, Socket socket)
    {
        Buffer = buffer;
        Socket = socket;
    }
}

