using System;
using System.Net.Sockets;
using System.Text;

public class LoginConnectionHandler
{
    public static void Handle(byte[] buffer, Socket handler)
    {
        
        var inputMessage = new InputMessage(buffer);

        var login = new LoginInput(inputMessage, handler);

        MessageQueue.Enqueue(new LoginCommand(login));
    }
}