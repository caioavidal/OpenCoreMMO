using System.Net.Sockets;

public interface ICommand
{

}

public abstract class Command:ICommand
{
    public Socket SocketHandler { get; set; }
}