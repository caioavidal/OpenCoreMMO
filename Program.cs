using System;
using System.Collections.Generic;
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

                List<Byte> response = new List<byte>();

                response.Add(0x14);
                response.AddRange(Encoding.UTF8.GetBytes("Welcome to the Ot Server"));
                response.Add(0x64);
                response.Add(1);
                response.AddRange(Encoding.UTF8.GetBytes("God"));
                response.AddRange(Encoding.UTF8.GetBytes("NetCore"));
                response.Add(Convert.ToByte(0));
                response.Add(Convert.ToByte(23));
                response.Add(Convert.ToByte(5));
              
                stream.Write(response.ToArray(), 0, response.ToArray().Length);
                client.Close();
            }
        }
    }
}
