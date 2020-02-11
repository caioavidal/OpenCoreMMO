using System;
using System.Net.Sockets;
using System.Text;

public class LoginInput : Input
{
    public string AccountName { get; set; }
    public string Password { get; set; }

    public int Version { get; set; }

    public LoginInput(InputMessage inputMessage, Socket socket) : base(inputMessage.Buffer, socket)
    {
        var packetPayload = inputMessage.GetUInt16();
        var tcpPayload = packetPayload + 2;
        var checkSum = inputMessage.GetUInt32();

        var request = inputMessage.GetByte();
        var os = inputMessage.GetUInt16();
        Version = inputMessage.GetUInt16();

        var files = inputMessage.GetBytes(12);

        var encryptedData = inputMessage.GetBytes(tcpPayload - inputMessage.BytesRead);


        var decryptedData = new InputMessage(RSA.Decrypt(encryptedData));


        LoadXtea(decryptedData);
        LoadAccount(decryptedData);
    }


    private void LoadAccount(InputMessage input)
    {
        AccountName = input.GetString();
        Password = input.GetString();
    }
    private void LoadXtea(InputMessage input)
    {
        for (int i = 0; i < 4; i++)
        {
            Xtea[i] = input.GetUInt32();
        }
    }
}