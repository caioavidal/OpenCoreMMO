using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace neoserver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 7171);

            server.Start();
            Console.WriteLine("Server has started on 127.0.0.1:7171.{0}Waiting for a connection...", Environment.NewLine);

            TcpClient client = server.AcceptTcpClient();

            Console.WriteLine("A client connected.");

            NetworkStream stream = client.GetStream();

            //enter to an infinite cycle to be able to handle every change in stream
            while (true)
            {
                while (!stream.DataAvailable) ;

                Byte[] bytes = new Byte[client.Available];

                stream.Read(bytes, 0, bytes.Length);

                var a = BitConverter.ToInt16(bytes,9); // protocolo

                String data = Encoding.UTF8.GetString(bytes);



              
            }
        }
    }
}
